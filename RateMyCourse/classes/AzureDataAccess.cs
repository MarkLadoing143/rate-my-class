using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Microsoft.WindowsAzure.MobileServices;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Android.Accounts;
using System.Net.Http;
using Android.Webkit;

namespace RateMyClass.classes
{
    public class AzureDataAccess
    {
        private MobileServiceClient MobileService;

        public AzureDataAccess()
        {
            MobileService = new MobileServiceClient("https://ratemyclass.azurewebsites.net");//Tylers database
			//MobileService = new MobileServiceClient("http://ratemycourse.azurewebsites.net");//Edberts test datebase
            CurrentPlatform.Init();

           // School school1 = new School { Name = "Mount Royal University", Location = "Calgary, AB" };
            //await MobileService.GetTable<School>().InsertAsync(school1);
        }

        //gets and returns a list of schools from DB
        public async Task<List<School>> getSchools()
        {
            IMobileServiceTable<School> schoolTable = MobileService.GetTable<School>();
            List<School> schoolsFromDB = await schoolTable.ToListAsync();
            return schoolsFromDB;
        }

        //gets and returns a list of classes for the provided school
        public async Task<List<Class>> getClasses(int schoolId)
        {
            IMobileServiceTable<Class> classTable = MobileService.GetTable<Class>();
            List<Class> classesFromDB = await classTable.Where(classItem => classItem.schoolId == schoolId).ToListAsync();
            return classesFromDB;
        }

        //adds the provided review to the database
        public async Task addReview(Review review)
        {
            await MobileService.GetTable<Review>().InsertAsync(review);
        }

        public async Task updateReview(Review review)
        {
            await MobileService.GetTable<Review>().UpdateAsync(review);
        }

        //returns a list of reviews for the given class ID
        public async Task<List<Review>> getReviews(string classID)
        {
            IMobileServiceTable<Review> classTable = MobileService.GetTable<Review>();
            List<Review> reviews = await classTable.Where(reviewItem => reviewItem.classId == classID).ToListAsync();
            return reviews;
        }

        public MobileServiceClient getMobileService()
        {
            return MobileService;
        }

        public async Task<bool> Authenticate(Activity requestedActivity)
        {
            Boolean success = false;
            if(Settings.IsLoggedIn)
            {
                success = true;
            }
            else
            {
                try
                {
                    await MobileService.LoginAsync(requestedActivity, MobileServiceAuthenticationProvider.Google);
                    Settings.UserId = MobileService.CurrentUser.UserId;
                    Settings.AuthToken = MobileService.CurrentUser.MobileServiceAuthenticationToken;
                    success = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
            return success;
        }

        public async Task<List<Location>> getLocations(string locationID)
        {
            IMobileServiceTable<Location> locationTable = MobileService.GetTable<Location>();
            List < Location > locations = await locationTable.Where(locationItem => locationItem.id == locationID).ToListAsync();
            return locations;
        }

        //checks if the user exists in the cache and loads it into the mobile service client if so
        public void loadUserFromCache()
        {
            if(Settings.IsLoggedIn)
            {
                MobileService.CurrentUser = new MobileServiceUser(Settings.UserId);
                MobileService.CurrentUser.MobileServiceAuthenticationToken = Settings.AuthToken;
            }
        }

        public async Task<UserInfo> getUserData()
        {
            return await MobileService.InvokeApiAsync<UserInfo>("UserDetails", HttpMethod.Get, null);
        }

        public async Task logout()
        {
            await MobileService.LogoutAsync();
            CookieManager.Instance.RemoveAllCookie();
            Settings.UserId = null;
            Settings.AuthToken = null;
        }
    }
}