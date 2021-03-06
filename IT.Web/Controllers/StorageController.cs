﻿using IT.Core.ViewModels;
using IT.Repository.WebServices;
using IT.Web.MISC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace IT.Web_New.Controllers
{
    [ExceptionLog]
    [Autintication]
    public class StorageController : Controller
    {
        WebServices webServices = new WebServices();
        List<StorageViewModel> storageViewModels = new List<StorageViewModel>();
        StorageViewModel StorageViewModel = new StorageViewModel();
        int CompanyId = 0;

        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                //List<StorageViewModel> storageViewModels2 = new List<StorageViewModel>();

                //PagingParameterModel pagingParameterModel = new PagingParameterModel
                //{ 
                //    pageNumber = 1,
                //    _pageSize = 1,
                //    Id = 0,
                //    PageSize = 100,
                // };
                //var StorageList = webServices.Post(pagingParameterModel, "Storage/All");

                //if (StorageList.StatusCode == System.Net.HttpStatusCode.Accepted)
                //{
                //    if (StorageList.Data != "[]")
                //    {
                //        storageViewModels = (new JavaScriptSerializer().Deserialize<List<StorageViewModel>>(StorageList.Data.ToString()));

                //        if (storageViewModels.Count > 0)
                //        {
                //            StorageViewModel storageViewModelObj = new StorageViewModel();

                //            foreach (var item in storageViewModels)
                //            {
                //                if (item.Action == true)
                //                {
                //                    storageViewModelObj.Id = item.Id;
                //                    storageViewModelObj.StockIn = item.StockIn;
                //                    storageViewModelObj.To = item.Source.ToLower() == "site" ? item.SiteName : item.TrafficPlateNumber;
                //                    if (item.Source == "Client Vehicle")
                //                    {
                //                        storageViewModelObj.To = item.Source.ToLower() == "site" ? item.SiteName : item.TrafficPlateNumberClient;
                //                    }
                //                    storageViewModelObj.ToSource = item.Source;
                //                    storageViewModelObj.UserName = item.UserName;
                //                }
                //                else
                //                {
                //                    storageViewModelObj.StockOut = item.StockOut;
                //                    storageViewModelObj.From = item.Source.ToLower() == "site" ? item.SiteName : item.TrafficPlateNumber;
                //                    storageViewModelObj.Source = item.Source;

                //                    storageViewModels2.Add(storageViewModelObj);
                //                    storageViewModelObj = new StorageViewModel();
                //                }
                //            }
                //        }
                //    }
                //    return View(storageViewModels2);
                //}
                //return View(storageViewModels);
                return View();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult All()
        {
            List<StorageViewModel> storageViewModels2 = new List<StorageViewModel>();
            try
            {
                CompanyId = Convert.ToInt32(Session["CompanyId"]);
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" +
                Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                string search = Request.Form.GetValues("search[value]")[0];
                //int skip = start != null ? Convert.ToInt32(start) : 0;

                int pageNumer = (Convert.ToInt32(start) / Convert.ToInt32(length)) + 1;

                PagingParameterModel pagingParameterModel = new PagingParameterModel();
                pagingParameterModel.pageNumber = pageNumer;
                pagingParameterModel._pageSize = pageSize;
                pagingParameterModel.PageSize = pageSize;
                pagingParameterModel.CompanyId = CompanyId;
                
                var StorageList = webServices.Post(pagingParameterModel, "Storage/AllWeb");
                int TotalCount = 0;
                if (StorageList.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    if (StorageList.Data != "[]")
                    {
                        storageViewModels = (new JavaScriptSerializer().Deserialize<List<StorageViewModel>>(StorageList.Data.ToString()));
                        TotalCount = storageViewModels[0].TotalRows;
                    }                   
                    return Json(new { draw = draw, recordsFiltered = TotalCount, recordsTotal = TotalCount, data = storageViewModels }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { draw = draw, recordsFiltered = 0, recordsTotal = 0, data = storageViewModels }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {                                       
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(StorageViewListModel storageViewListModel)
        {
            try
            {
                if(storageViewListModel.storageViewModels != null && storageViewListModel.storageViewModels.Count > 1)
                {
                    if (storageViewListModel.storageViewModels[1].StockOut == 0 || storageViewListModel.storageViewModels[0].StockIn == 0)
                    {
                        //return View(storageViewListModel.storageViewModels);
                        return Json("NoData", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var value = DateTime.Now.ToFileTime().ToString();

                        List<StorageViewModel> storageViewModels1 = new List<StorageViewModel>();
                        storageViewModels1 = storageViewListModel.storageViewModels;

                        storageViewModels1[0].CreatedBy = Convert.ToInt32(Session["UserId"]);
                        storageViewModels1[1].CreatedBy = Convert.ToInt32(Session["UserId"]);
                        storageViewModels1[0].uniques = value;
                        storageViewModels1[1].uniques = value;
                        var result = webServices.Post(storageViewModels1, "Storage/StorageAdd");
                        if (result.StatusCode == System.Net.HttpStatusCode.Accepted)
                        {
                            if(result.Data != "[]")
                            {
                                var storage  = (new JavaScriptSerializer()).Deserialize<StorageViewModel>(result.Data);
                                return Json("success", JsonRequestBehavior.AllowGet);
                            }    
                            else
                            {
                                return Json("NoData", JsonRequestBehavior.AllowGet);
                            }                            
                        }
                        else
                        {
                            return Json("NoData", JsonRequestBehavior.AllowGet);
                        }
                    }                    
                }
                else
                {
                    return Json("NoData", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Edit(int Id)
        {
            try
            {
                StorageViewModel.Id = Id;
                var result = webServices.Post(StorageViewModel, "Storage/Edit");
                if (result.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    if (result.Data != "[]")
                    {
                        storageViewModels = (new JavaScriptSerializer().Deserialize<List<StorageViewModel>>(result.Data.ToString()));
                    }
                }
                ProductController productController = new ProductController();
                SiteController siteController = new SiteController();
                VehicleController vehicleController = new VehicleController();
                AwfVehicleController awfVehicleController = new AwfVehicleController();
                int CompanyId = Convert.ToInt32(Session["CompanyId"]);

                ViewBag.AdminVehicles = awfVehicleController.AdminVehicles();
                ViewBag.Vehicles = vehicleController.Vehicles();
                ViewBag.Sites = siteController.SitesList(CompanyId);
                ViewBag.Products = productController.Products();
                return View(storageViewModels);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult Update(StorageViewListModel storageViewListModel)
        {
            try
            {
                // var value = DateTime.Now.ToFileTime().ToString();
                List<StorageViewModel> storageViewModels1 = new List<StorageViewModel>();
                storageViewModels1 = storageViewListModel.storageViewModels;

                storageViewModels1[0].UpdateBy = Convert.ToInt32(Session["UserId"]);
                storageViewModels1[1].UpdateBy = Convert.ToInt32(Session["UserId"]);

                var result = webServices.Post(storageViewModels1, "Storage/StorageUpdate");
                if (result.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    int k = (new JavaScriptSerializer()).Deserialize<int>(result.Data);
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Failed", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception)
            {
                return Json("Failed", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult UploadDocumentsAdd(UploadDocumentsViewModel uploadDocumentsViewModel, HttpPostedFileBase FileUrl)
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    var file = FileUrl;

                    using (HttpClient client = new HttpClient())
                    {
                        using (var content = new MultipartFormDataContent())
                        {
                            byte[] fileBytes = new byte[file.InputStream.Length + 1];
                            file.InputStream.Read(fileBytes, 0, fileBytes.Length);
                            var fileContent = new ByteArrayContent(fileBytes);
                            fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("FileUrl") { FileName = file.FileName };
                            content.Add(fileContent);
                            content.Add(new StringContent("ClientDocs"), "ClientDocs");
                            string UserId = Session["UserId"].ToString();
                            content.Add(new StringContent(UserId), "CreatedBy");
                            content.Add(new StringContent(uploadDocumentsViewModel.StorageId.ToString()), "StorageId");
                            content.Add(new StringContent(uploadDocumentsViewModel.FilesName ?? "Unknown"), "FilesName");
                            var result = webServices.PostMultiPart(content, "UploadDocuments/UploadDocumentsAdd", true);
                            if (result.StatusCode == System.Net.HttpStatusCode.Accepted)
                            {
                                return Redirect(nameof(Index));
                            }
                            else
                            {
                                return Redirect(nameof(Index));
                            }
                        }
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}