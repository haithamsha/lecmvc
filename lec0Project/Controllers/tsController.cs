using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using RHRModel;
using Newtonsoft.Json;
using RHRView.Helpers;
using RHR.BLL.PermissionRequestServices;
using RHR.ViewModels.VMS;
using RHRView.Controllers;
using System.Threading;
using System.Text;
using System.Web.Script.Serialization;

namespace RHRView.Controllers
{
    [Internationalization]
    public class PermissionController : Controller
    {

        [HttpGet]
        public async Task<ActionResult> PermissionRequest(int? id = null)
        {
            // Get companyCode and empcode from session
            LoginInfoVm loginInfo = new LoginInfoVm();

            if (Session["loginInfo"] != null)
            {
                loginInfo = (LoginInfoVm)Session["loginInfo"];
            }
            else
            {
                return RedirectToAction("index", "login");
            }



            if (id != null)
            {

                ViewBag.CanClear = "false";
                ViewBag.CanSave = "false";

                // Get request details
                HttpResponseMessage pDetilsApi = await SharedAPIsHelper.GetApiData($"api/PermissionRequest/PermissionDetails/{id}/{loginInfo.CompanyCode}", SharedAPIsHelper.BaseUrl);

                if (pDetilsApi.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var response_details = pDetilsApi.Content.ReadAsStringAsync().Result;
                    var detailsData = JsonConvert.DeserializeObject<PermissionRequestVM>(response_details);
                    fillDropDownLists();

                    // Get some Previous permission data
                    string[] _prevData = await getPreviousData(loginInfo.EmpCode, loginInfo.CompanyCode, id.Value);
                    if (_prevData != null)
                    {
                        detailsData.PreviousHrs = _prevData[0];
                        detailsData.PreviousSerial = _prevData[1];
                    }
                    else
                    {
                        detailsData.PreviousHrs = "0";
                        detailsData.PreviousSerial = "";
                    }



                    // Get empcode with employee id
                    detailsData.empcode = detailsData.EmployeeId;
                    // Disable cancel button if the request is not pending
                    if (detailsData.status == "0")
                    {
                        ViewBag.CanCanceled = "true";
                    }




                    return View(detailsData);
                }
                fillDropDownLists();
                return View();

            }

            // Prepare the view model
            PermissionRequestVM vm = new PermissionRequestVM();

            //fill document date to todays date
            vm.DocumentDate = DateTime.Now.ToString("dd-MM-yyyy");

            // Get some Previous permission data
            string[] prevData = await getPreviousData(loginInfo.EmpCode, loginInfo.CompanyCode);
            if (prevData != null)
            {
                vm.PreviousHrs = prevData[0];
                vm.PreviousSerial = prevData[1];
            }

            // Get the employee shift
            //var _shiftData = await getEmployeeShift(loginInfo.EmpCode, DateTime.Now, loginInfo.CompanyCode);
            //if(_shiftData != null)
            //{
            //    vm.StartShift = _shiftData.StartShift.ToShortTimeString();
            //    vm.EndShift = _shiftData.EndShift.ToShortTimeString();
            //}

            vm.PermissionDate = DateTime.Now.ToString("MM-dd-yyyy");
            fillDropDownLists();
            return View(vm);


        }

        [HttpPost]
        public ActionResult PermissionRequest(PermissionRequestVM vm)
        {
            // Get companyCode and empcode from session
            LoginInfoVm loginInfo = new LoginInfoVm();

            if (Session["loginInfo"] != null)
            {
                loginInfo = (LoginInfoVm)Session["loginInfo"];
            }
            else
            {
                return RedirectToAction("index", "login");
            }

            using (var client = new HttpClient())
            {
                //Set Latness and early left with true by default
                vm.incloudeEarlyLeft = true;
                vm.inclouderLateness = true;

                // Set AutoApproved with false by default.
                vm.AutoApproved = false;
                // Set isSecondShift false by default
                if (vm.IsSecoundShift == null) vm.IsSecoundShift = false;

                vm.UserID = loginInfo.UserId;

                vm.compcode = loginInfo.CompanyCode;
                client.BaseAddress = new Uri(SharedAPIsHelper.BaseUrl);

                client.DefaultRequestHeaders.Clear();
                var postTask = client.PostAsJsonAsync<PermissionRequestVM>($"{SharedAPIsHelper.BaseUrl}/{SharedAPIsHelper.PermissionSaveUrl}", vm);

                postTask.Wait();
                var result = postTask.Result;

                //Checking the response is successful or not which is sent using HttpClient  
                if (result.IsSuccessStatusCode)
                {
                    fillDropDownLists();
                    var _response = result.Content.ReadAsStringAsync().Result;
                    var _msg = JsonConvert.DeserializeObject<object>(_response);
                    ViewBag.resultMessage = _msg;
                    return View(vm);

                }
            }

            fillDropDownLists();
            return View(vm);

        }

        private async Task<string[]> getPreviousData(string empCode, string companyCode, int? serial = null)
        {
            HttpResponseMessage prevApi = await SharedAPIsHelper.GetApiData($"api/PermissionRequest/GetPreviousHoursAndSerial/{empCode}/{companyCode}/{serial}", SharedAPIsHelper.BaseUrl);

            if (prevApi.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var prevResponse = prevApi.Content.ReadAsStringAsync().Result;
                var prevData = JsonConvert.DeserializeObject<string[]>(prevResponse);
                return prevData;
            }
            return null;
        }

        public ActionResult getEmployeeShift(string empCode, DateTime date)
        {
            try
            {
                // Get companyCode and empcode from session
                LoginInfoVm loginInfo = new LoginInfoVm();

                if (Session["loginInfo"] != null)
                {
                    loginInfo = (LoginInfoVm)Session["loginInfo"];
                }
                else
                {
                    return RedirectToAction("index", "login");
                }



                string _date = string.Format("{0:MM-dd-yyyy}", date);

                HttpResponseMessage shiftApi = SharedAPIsHelper.GetApiData_($"api/PermissionRequest/GetEmployeeShift/{empCode}/{_date}/{loginInfo.CompanyCode}");

                if (shiftApi.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var shiftResponse = shiftApi.Content.ReadAsStringAsync().Result;
                    var shiftData = JsonConvert.DeserializeObject<EmpShiftVM>(shiftResponse);
                    return Json(shiftData, JsonRequestBehavior.AllowGet);
                }

                return Json(RHR.ViewModels.Resources.General.error, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json($"{RHR.ViewModels.Resources.General.error} {ex.Message}", JsonRequestBehavior.AllowGet);
            }
        }

        void fillDropDownLists()
        {
            // Fill Employees combo dropdownlist

            var EmpApi = SharedAPIsHelper.FillEmployeessDropDownList();

            var Response_Emp = EmpApi.Content.ReadAsStringAsync().Result;
            var empsData = JsonConvert.DeserializeObject<List<EmployeeDataVM>>(Response_Emp);

            if (EmpApi.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (Thread.CurrentThread.CurrentCulture.Name == "en-US")
                {
                    ViewBag.EmpName = new SelectList(empsData, "empcode", "EmpName");
                }

                else
                {
                    ViewBag.EmpName = new SelectList(empsData, "empcode", "EmpARName");
                }
            }

            else
            {
                ModelState.AddModelError("EmpCode", RHR.ViewModels.Resources.General.error);
            }

            // Fill permission types dropdown.
            var PermissionTypeApi = SharedAPIsHelper.FillPermissionTypesDropDownList();

            var Response_PermissionType = PermissionTypeApi.Content.ReadAsStringAsync().Result;
            var permissionsData = JsonConvert.DeserializeObject<List<object>>(Response_PermissionType);

            if (PermissionTypeApi.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (Thread.CurrentThread.CurrentCulture.Name == "en-US")
                {
                    ViewBag.permissiontype = new SelectList(permissionsData, "PermissionCode", "PermissionName");
                }

                else
                {
                    ViewBag.permissiontype = new SelectList(permissionsData, "PermissionCode", "PermissionNameAr");
                }
            }

            else
            {
                ModelState.AddModelError("PermissionType", RHR.ViewModels.Resources.General.error);
            }


            // /Fill Permission Status dropdown


            var PermissionStatusApi = SharedAPIsHelper.FillPermissionStatusDropDownList_Request();

            var Response_PermissionStatus = PermissionStatusApi.Content.ReadAsStringAsync().Result;
            var permissionsStausData = JsonConvert.DeserializeObject<List<object>>(Response_PermissionStatus);

            if (PermissionTypeApi.StatusCode == System.Net.HttpStatusCode.OK)
            {
                if (Thread.CurrentThread.CurrentCulture.Name == "en-US")
                {
                    ViewBag.PermissionStatus = new SelectList(permissionsStausData, "status", "statusName");
                }

                else
                {
                    ViewBag.PermissionStatus = new SelectList(permissionsStausData, "status", "statusARName");
                }
            }

            else
            {
                ModelState.AddModelError("status", RHR.ViewModels.Resources.General.error);
            }
        }



        public async Task<ActionResult> EmpMissionPermission()
        {
            // Get companyCode and empcode from session
            LoginInfoVm loginInfo = new LoginInfoVm();

            if (Session["loginInfo"] != null)
            {
                loginInfo = (LoginInfoVm)Session["loginInfo"];
            }
            else
            {
                return RedirectToAction("index", "login");
            }

            List<EmployeeDataVM> _EmpViewModel = new List<EmployeeDataVM>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(SharedAPIsHelper.BaseUrl);

                client.DefaultRequestHeaders.Clear();
                //Define request data format  
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //Sending request to find web api REST service resource GetAllEmployees using HttpClient  
                HttpResponseMessage Res = await client.GetAsync($"api/RptMissionPermission/EmployeeData/${loginInfo.CompanyCode}/${loginInfo.EmpCode}");
                //HttpResponseMessage Rescity = await client.GetAsync("api/City/GetCity");

                if (Res.IsSuccessStatusCode)
                {
                    //Storing the response details recieved from web api   
                    var Response = Res.Content.ReadAsStringAsync().Result;
                    var empsData = JsonConvert.DeserializeObject<List<EmployeeDataVM>>(Response);

                    ViewBag.EmpName = new SelectList(empsData, "empcode", "EmpARName");

                    //Deserializing the response recieved from web api and storing into the Employee list  
                    //CLI = JsonConvert.DeserializeObject<List<POSITION>>(Response);

                }
            }

            return View();
        }


        //public ActionResult Fill()
        //{
        //    var emp = new EmpMissionPermissionVM();

        //    var emmps=
        //}

        public ActionResult Details()
        {
            return View();
        }


        public async Task<ActionResult> PermissionApproval(string EmpCode, int? Approved = null, int? PermissionType = null, bool? ApproveAll = null, DateTime? FromTime = null, DateTime? ToTime = null)
        {

            // Get companyCode and empcode from session
            LoginInfoVm loginInfo = new LoginInfoVm();

            if (Session["loginInfo"] != null)
            {
                loginInfo = (LoginInfoVm)Session["loginInfo"];
            }
            else
            {
                return RedirectToAction("index", "login");
            }

            // Fill Employes dropdownlist
            HttpResponseMessage empApi = new HttpResponseMessage();
            try
            {
                empApi = await SharedAPIsHelper.GetApiData(SharedAPIsHelper.EmployeesServiceUrl, SharedAPIsHelper.BaseUrl);
            }
            catch (Exception)
            {

                ModelState.AddModelError("EmpCode", RHR.ViewModels.Resources.General.error);
                return View();
            }


            if (empApi.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // Create viewbag with the result of the empApi
                var Response_empApi = empApi.Content.ReadAsStringAsync().Result;
                var empData = JsonConvert.DeserializeObject<List<EmployeeDataVM>>(Response_empApi);

                // Create view bag according to the localizations
                if (Thread.CurrentThread.CurrentCulture.Name == "en-US")
                {
                    ViewBag.Employees = new SelectList(empData, "empcode", "EmpName", 1);
                }

                else
                {
                    ViewBag.Employees = new SelectList(empData, "empcode", "EmpARName", 1);
                }
            }
            else
            {
                ModelState.AddModelError("EmpCode", RHR.ViewModels.Resources.General.error);
                return View();
            }


            // Fill Permission Type Dropdownlist

            HttpResponseMessage PermissionTypeApi = await SharedAPIsHelper.GetApiData(SharedAPIsHelper.PermissionTypeUrl, SharedAPIsHelper.BaseUrl);

            if (PermissionTypeApi.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // Create viewbag with the result of the empApi
                var Response_PermissionTypeApi = PermissionTypeApi.Content.ReadAsStringAsync().Result;
                var PermissionTypeData = JsonConvert.DeserializeObject<List<object>>(Response_PermissionTypeApi);

                // Create view bag according to the localizations
                if (Thread.CurrentThread.CurrentCulture.Name == "en-US")
                {
                    ViewBag.PermissionTypes = new SelectList(PermissionTypeData, "PermissionCode", "PermissionName");
                }

                else
                {
                    ViewBag.Employees = new SelectList(PermissionTypeData, "PermissionCode", "PermissionNameAr");
                }
            }
            else
            {
                ModelState.AddModelError("PermissionType", RHR.ViewModels.Resources.General.error);
            }


            // Fill Approved status Dropdownlist
            HttpResponseMessage PermissionStatusApi = await SharedAPIsHelper.GetApiData(SharedAPIsHelper.PermissinoStatusUrl, SharedAPIsHelper.BaseUrl);
            List<PermissionStatusVM> pstatus = new List<PermissionStatusVM>();

            if (PermissionTypeApi.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // Create viewbag with the result of the empApi
                var Response_PermissionStatusApi = PermissionStatusApi.Content.ReadAsStringAsync().Result;
                var PermissionStatusData = JsonConvert.DeserializeObject<List<PermissionStatusVM>>(Response_PermissionStatusApi);


                pstatus = PermissionStatusData;

                // Create view bag according to the localizations
                if (Thread.CurrentThread.CurrentCulture.Name == "en-US")
                {
                    ViewBag.PermissionStatus = new SelectList(PermissionStatusData, "status", "statusName");
                }

                else
                {
                    ViewBag.Employees = new SelectList(PermissionStatusData, "status", "statusARName");
                }
            }
            else
            {
                ModelState.AddModelError("Approved", RHR.ViewModels.Resources.General.error);
            }

            // Get approvals data according to the current employee from the rest api.
            PermissionRequestServices _pService = new PermissionRequestServices();


            string approvalsUrl = $"api/PermissionRequest/PermissionApproveData/{-1}/{0}/{loginInfo.CompanyCode}/{loginInfo.UserId}";

            if (EmpCode != null)
            {
                approvalsUrl = $"api/PermissionRequest/PermissionApproveData/{EmpCode}/{-1}/{4}/{loginInfo.CompanyCode}/{loginInfo.UserId}";
            }

            if (EmpCode != "" && Approved != null)
            {
                approvalsUrl = $"api/PermissionRequest/PermissionApproveData/{EmpCode}/{-1}/{Approved}/{loginInfo.CompanyCode}/{loginInfo.UserId}";
            }

            else if (EmpCode != "" && FromTime != null && ToTime != null)
            {

                approvalsUrl = $"api/PermissionRequest/PermissionApproveData/{EmpCode}/{-1}/{4}/{loginInfo.CompanyCode}/{loginInfo.UserId}/{FromTime}/{ToTime}";
            }

            else if (EmpCode != "" && PermissionType != null)
            {
                approvalsUrl = $"api/PermissionRequest/PermissionApproveData/{EmpCode}/{PermissionType}/{4}/{loginInfo.CompanyCode}/{loginInfo.UserId}";
            }
            else if (EmpCode != "" && PermissionType != null && Approved != null)
            {
                approvalsUrl = $"api/PermissionRequest/PermissionApproveData/{EmpCode}/{PermissionType}/{Approved}/{loginInfo.CompanyCode}/{loginInfo.UserId}";
            }
            else if (EmpCode != "" && PermissionType != null && Approved != null && FromTime != null && ToTime != null)
            {
                approvalsUrl = $"api/PermissionRequest/PermissionApproveData/{EmpCode}/{PermissionType}/{Approved}/{loginInfo.CompanyCode}/{loginInfo.UserId}/{FromTime}/{ToTime}";
            }




            HttpResponseMessage permissionsApi = await SharedAPIsHelper.GetApiData(approvalsUrl, SharedAPIsHelper.BaseUrl);

            if (permissionsApi.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // Create viewbag with the result of the empApi
                var Response_PermissionsApprovals = permissionsApi.Content.ReadAsStringAsync().Result;

                var permissions = JsonConvert.DeserializeObject<List<PermissionApprovalVM>>(Response_PermissionsApprovals);

                foreach (var item in permissions)
                {
                    item.Status = new SelectList(pstatus, "status", "statusName", item.ApprovePerReq);
                }
                return View(permissions);
            }
            return View();
        }

        [HttpPost]
        public JsonResult PermissionApproval(int configStepProcessId, int status, string notes)
        {
            using (var client = new HttpClient())
            {

                //// Temporary set the user id 'Right' for demo test
                //vm.UserID = "Right";

                client.BaseAddress = new Uri(SharedAPIsHelper.BaseUrl);

                client.DefaultRequestHeaders.Clear();
                var postTask = client.PostAsync($"{SharedAPIsHelper.BaseUrl}/api/PermissionRequest/UpdateStatus/{configStepProcessId}/{status}/{notes}", null);

                postTask.Wait();
                var result = postTask.Result;

                //Checking the response is successful or not which is sent using HttpClient  
                string _resultMessage = "";
                if (result.IsSuccessStatusCode)
                {
                    var _response = result.Content.ReadAsStringAsync().Result;
                    var _msg = JsonConvert.DeserializeObject<object>(_response);
                    _resultMessage = _msg.ToString();
                    ViewBag.resultMessage = _resultMessage;
                    return Json(_msg, JsonRequestBehavior.AllowGet);
                }
                return Json(_resultMessage, JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> PermissionRequestSerials(string id = "")
        {
            // Get companyCode and empcode from session
            LoginInfoVm loginInfo = new LoginInfoVm();

            if (Session["loginInfo"] != null)
            {
                loginInfo = (LoginInfoVm)Session["loginInfo"];
            }
            else
            {
                return RedirectToAction("index", "login");
            }

            id = id == "" ? loginInfo.EmpCode : id;

            HttpResponseMessage permissionsGridApi = await SharedAPIsHelper.GetApiData($"api/PermissionRequest/SerialGridData/{loginInfo.CompanyCode}/{id}", SharedAPIsHelper.BaseUrl);
            if (permissionsGridApi.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // Create viewbag with the result of the permissions serials
                var Response_PermissionsRequestSerial = permissionsGridApi.Content.ReadAsStringAsync().Result;
                var permissions = JsonConvert.DeserializeObject<List<PermissionRequestVM>>(Response_PermissionsRequestSerial);
                return View(permissions);
            }

            return View();

        }

        public ActionResult Cancel(string serial, string empCode)
        {

            // Get companyCode and empcode from session
            LoginInfoVm loginInfo = new LoginInfoVm();

            if (Session["loginInfo"] != null)
            {
                loginInfo = (LoginInfoVm)Session["loginInfo"];
            }
            else
            {
                return RedirectToAction("index", "login");
            }

            using (var client = new HttpClient())
            {

                //// Temporary set the user id 'Right' for demo test
                //vm.UserID = "Right";

                client.BaseAddress = new Uri(SharedAPIsHelper.BaseUrl);

                client.DefaultRequestHeaders.Clear();
                var postTask = client.PostAsync($"{SharedAPIsHelper.BaseUrl}/api/PermissionRequest/Cancel/{loginInfo.CompanyCode}/{3}/{serial}/{empCode}", null);

                postTask.Wait();
                var result = postTask.Result;

                //Checking the response is successful or not which is sent using HttpClient  
                string _resultMessage = "";
                if (result.IsSuccessStatusCode)
                {
                    var _response = result.Content.ReadAsStringAsync().Result;
                    var _msg = JsonConvert.DeserializeObject<object>(_response);
                    _resultMessage = _msg.ToString();
                    ViewBag.resultMessage = _resultMessage;
                    return Json(_msg, JsonRequestBehavior.AllowGet);
                }
                return Json(_resultMessage, JsonRequestBehavior.AllowGet);
            }
        }
    }
}




