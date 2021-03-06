﻿using IT.Core.ViewModels;
using IT.Repository.WebServices;
using IT.Web.MISC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace IT.Web_New.Controllers
{
    [Autintication]
    [ExceptionLog]
    public class CountryController : Controller
    {
        WebServices webServices = new WebServices();
        readonly CountryViewModel countryViewModel = new CountryViewModel();
        List<CountryViewModel> countryViewModels = new List<CountryViewModel>();
        readonly StateViewModel StateViewModel = new StateViewModel();
        List<StateViewModel> stateViewModels = new List<StateViewModel>();
        readonly CityViewModel CityViewModel = new CityViewModel();
        List<CityViewModel> cityViewModels = new List<CityViewModel>();

        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                var countryList = webServices.Post(new CountryViewModel(), "Country/GetAll");

                if (countryList.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    countryViewModels = (new JavaScriptSerializer().Deserialize<List<CountryViewModel>>(countryList.Data.ToString()));
                }
                if (Request.IsAjaxRequest())
                {
                    return Json(countryViewModels, JsonRequestBehavior.AllowGet);
                }
                return View(countryViewModels);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult StateAll()
        {
            try
            {
                var stateList = webServices.Post(new StateViewModel(), "Country/StateAll");

                if (stateList.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    stateViewModels = (new JavaScriptSerializer().Deserialize<List<StateViewModel>>(stateList.Data.ToString()));
                }
                if (Request.IsAjaxRequest())
                {
                    return Json(stateViewModels, JsonRequestBehavior.AllowGet);
                }
                return View(stateViewModels);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult AddUpdateState()
        {
            CountryController countryController = new CountryController();
            ViewBag.Countries = countryController.Countries();
            return View(new StateViewModel());
        }

        [HttpPost]
        public ActionResult AddUpdateState(StateViewModel stateViewModel)
        {
            try
            {
                //stateViewModels.Insert(0, stateViewModel);
                var stateResult = new ServiceResponseModel();
                if (stateViewModel.Id < 1)
                {
                    stateResult = webServices.Post(stateViewModel, "Country/AddUpdateState");
                }
                else
                {
                    stateResult = webServices.Post(stateViewModel, "Country/AddUpdateState");
                }
                if (stateResult.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    var reuslt = (new JavaScriptSerializer().Deserialize<int>(stateResult.Data));
                    return RedirectToAction(nameof(StateAll));
                }
                return View(stateResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult EditState(int Id, String Name, int CountryId = 0)
        {
            StateViewModel stateViewModel = new StateViewModel
            { 
                Id = Id,
                States = Name,
                CountryId = CountryId,
            };
            CountryController countryController = new CountryController();
            ViewBag.Countries = countryController.Countries();

            return View("AddUpdateState", stateViewModel);
        }

        [NonAction]
        public List<StateViewModel> States()
        {
            try
            {
                var stateList = webServices.Post(new ProductViewModel(), "Country/StateAll");

                if (stateList.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    stateViewModels = (new JavaScriptSerializer().Deserialize<List<StateViewModel>>(stateList.Data.ToString()));

                }
                return stateViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new CountryViewModel());
        }

        [HttpPost]
        public ActionResult Create(CountryViewModel countryViewModel)
        {
            try
            {
                countryViewModels.Insert(0, countryViewModel);
                var CountryResult = new ServiceResponseModel();
                if (countryViewModel.Id < 1)
                {
                    CountryResult = webServices.Post(countryViewModels, "Country/Add");
                }
                else
                {
                    CountryResult = webServices.Post(countryViewModel, "Country/Update");
                }
                if (CountryResult.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    var reuslt = (new JavaScriptSerializer().Deserialize<int>(CountryResult.Data));
                    return RedirectToAction(nameof(Index));
                }
                return View(CountryResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult Edit(int Id, String Name)
        {
            CountryViewModel countryViewModel = new CountryViewModel
            {
                Id = Id,
                CountryName = Name,
            };
            return View("Create", countryViewModel);
        }

        [NonAction]
        public List<CountryViewModel> Countries()
        {
            try
            {
                var countryList = webServices.Post(new ProductViewModel(), "Country/All");

                if (countryList.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    countryViewModels = (new JavaScriptSerializer().Deserialize<List<CountryViewModel>>(countryList.Data.ToString()));

                }
                return countryViewModels;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult CityAll(string StateName)
        {
            try
            {
                var cityList = webServices.Post(new CityViewModel(), "Country/CityAll");

                if (cityList.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    cityViewModels = (new JavaScriptSerializer().Deserialize<List<CityViewModel>>(cityList.Data.ToString()));
                }
                if (Request.IsAjaxRequest())
                {
                    if(StateName != null)
                    {
                        cityViewModels = cityViewModels.Where(x => x.StateName == StateName).ToList();
                    }

                    return Json(cityViewModels, JsonRequestBehavior.AllowGet);
                }
                return View(cityViewModels);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet]
        public ActionResult AddUpdateCity()
        {
            CountryController countryController = new CountryController();
            ViewBag.States = countryController.States();
            return View(new CityViewModel());
        }

        [HttpPost]
        public ActionResult AddUpdateCity(CityViewModel cityViewModel)
        {
            try
            {
                //stateViewModels.Insert(0, stateViewModel);
                var cityResult = new ServiceResponseModel();
                if (cityViewModel.Id < 1)
                {
                    cityResult = webServices.Post(cityViewModel, "Country/AddUpdateCity");
                }
                else
                {
                    cityResult = webServices.Post(cityViewModel, "Country/AddUpdateCity");
                }
                if (cityResult.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    var reuslt = (new JavaScriptSerializer().Deserialize<int>(cityResult.Data));
                    return RedirectToAction(nameof(CityAll));
                }
                return View(cityResult);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public ActionResult EditCity(int Id, String Name, int StateId = 0)
        {
            CityViewModel cityViewModel = new CityViewModel
            { 
                Id = Id,
                CityName = Name,
                StateId = StateId,
            };
        CountryController countryController = new CountryController();
            ViewBag.States = countryController.States();

            return View("AddUpdateCity", cityViewModel);
        }

    }
}