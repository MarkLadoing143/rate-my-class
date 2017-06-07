using System;
using Android.App;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Threading;
using System.Collections;
using Android.Content;
using Android.Util;
using Microsoft.WindowsAzure.MobileServices;
using RateMyClass.classes;
using System.Collections.Generic;
using System.Threading.Tasks;

//This stuff deals with the toolbar
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using ActionBar = Android.Support.V7.App.ActionBar;

//Drawer Layout stuff
using Android.Support.V4.Widget;
using Android.Content.PM;

namespace RateMyClass
{
	//[Activity(Label = "Rate My Class", Icon = "@drawable/staricon", Theme = "@android:style/Theme.Black.NoTitleBar")]
	[Activity(Label = "Rate My Class", Icon = "@drawable/staricon", ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : AppCompatActivity
	{
		private ListView schoolListView;
		private SearchView searchBar;
		private ProgressBar spinner;
		public static List<School> schools;
		//Include this for menu
		public static AzureDataAccess azureDataAccess;
		//Menu and Drawer stuff
		private Bundle cBundle;
		private ActionBar actionBar;
		private Toolbar toolbar;
		private ActionBarToggle mDrawerToggle;
		private DrawerLayout mDrawerLayout;
		private ListView mDrawerList;
        private List<string> mDrawerListItems;
        public static int scoolId2 = 0;


        protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			//any preprocessing before displaying main app screen put here
			azureDataAccess = new AzureDataAccess();
            azureDataAccess.loadUserFromCache();

			//switch from splash to our main view once done loading
			SetContentView(Resource.Layout.Main);

			//initialize action bar
			initActionBar();

			findViews();
			handleEvents();

			//retreive schools from DB and display them
			schools = await azureDataAccess.getSchools();

			//update list view with data
			displayAvailableSchools(schools);

			//hide loading spinner
			spinner.Visibility = Android.Views.ViewStates.Gone;
		}

		private void initActionBar()
		{
			//Toolbar stuff
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

			if (toolbar != null)
			{


				//initialize the drawer
				mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout_Main);
				mDrawerList = FindViewById<ListView>(Resource.Id.drawerListView_Main);

				//Toolbar will now take on default Action Bar characteristics
				SetSupportActionBar(toolbar);
				actionBar = SupportActionBar;


				mDrawerToggle = new ActionBarToggle(this, mDrawerLayout, Resource.String.drawer_opened, Resource.String.drawer_closed_schoolSearch);


				mDrawerLayout.AddDrawerListener(mDrawerToggle);

				//Set Action Bar Title
				actionBar.Title = "Select School";

				actionBar.SetDisplayHomeAsUpEnabled(true);
				actionBar.SetHomeButtonEnabled(true);
				actionBar.SetDisplayShowHomeEnabled(true);
				actionBar.SetDisplayShowTitleEnabled(true);
				mDrawerToggle.SyncState();

				if (cBundle != null)
				{
					if (cBundle.GetString("DrawerState") == "Opened")
					{
						SupportActionBar.SetTitle(Resource.String.drawer_closed_schoolSearch);
					}
				}
				else {
					SupportActionBar.SetTitle(Resource.String.drawer_closed_schoolSearch);
				}


				//Setting Up items in list
                mDrawerListItems = new List<string>(Resources.GetStringArray(Resource.Array.drawer_list));
                if (Settings.IsLoggedIn)
                {
                    mDrawerListItems.Add("Logout");
                }
                else
                {
                    mDrawerListItems.Add("Login");
                }
                ArrayAdapter<string> drawListAdapter = new ArrayAdapter<string>(this, Resource.Layout.menuListItem, mDrawerListItems);
				mDrawerList.Adapter = drawListAdapter;

			}
		}


		//Add listener to drawer toggle
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			mDrawerToggle.OnOptionsItemSelected(item);
			return base.OnOptionsItemSelected(item);
		}


		//Deal with orientation change for title of drawer
		protected override void OnSaveInstanceState(Bundle outState)
		{
			if (mDrawerLayout.IsDrawerOpen((int)GravityFlags.Left))
			{
				outState.PutString("DrawerState", "Opened");
			}
			else {
				outState.PutString("DrawerState", "Closed");
			}
			base.OnSaveInstanceState(outState);
		}


        //Deal with the items on the drawer list
        //runs when the user clicks an item in the list
        private async void DrawerListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //get which class the user clicked on
            int clickedActionId = e.Position;


            //Get list of stuff that needs to be added and prebuild a bundle
            Bundle bundle = new Bundle();

            if (base.Intent.HasExtra("selectedSchoolId"))
            {
                bundle.PutInt("selectedSchoolId", base.Intent.GetIntExtra("selectedSchoolId", -1));
            }

            if ((base.Intent.HasExtra("selectedClassId")))
            {
                bundle.PutInt("selectedClassId", base.Intent.GetIntExtra("selectedClassId", -1));
            }

            if (base.Intent.HasExtra("selectedLocation"))
            {
                bundle.PutString("selectedLocation", Intent.GetStringExtra("selectedLocation"));
            }


            if (clickedActionId == 0)
            {
                var intent = new Intent(this, typeof(MainActivity));
                intent.PutExtras(bundle);
                StartActivity(intent);
            }
            else if (clickedActionId == 1)
            {
                if ((base.Intent.HasExtra("selectedSchoolId")))
                {
                    var intent = new Intent(this, typeof(SearchClassActivity));
                    intent.PutExtras(bundle);
                    StartActivity(intent);
                }
                else
                {
                    Toast.MakeText(this, "No School Selected", ToastLength.Short).Show();
                }
            }
            else if (clickedActionId == 2)
            {
                if ((base.Intent.HasExtra("selectedClassId")))
                {
                    var intent = new Intent(this, typeof(ClassViewActivity));
                    intent.PutExtras(bundle);
                    StartActivity(intent);
                }
                else
                {
                    Toast.MakeText(this, "No Class Selected", ToastLength.Short).Show();
                }
            }
            else if (mDrawerListItems[clickedActionId] == "Logout")
            {
                //Logout user
                await azureDataAccess.logout();
                //refresh toolbar
                initActionBar();
                Toast.MakeText(this, "Logged Out", ToastLength.Short).Show();
            }
            else if (mDrawerListItems[clickedActionId] == "Login")
            {
                //Login user
                await azureDataAccess.Authenticate(this);
                //refresh toolbar
                initActionBar();
                Toast.MakeText(this, "Logged In", ToastLength.Short).Show();
            }
            else
            {
                Toast.MakeText(this, "Unknown Option Selected", ToastLength.Short).Show();
            }
        }


        private void findViews()
		{
			schoolListView = FindViewById<ListView>(Resource.Id.schoolList);
			searchBar = FindViewById<SearchView>(Resource.Id.searchView1);
			spinner = FindViewById<ProgressBar>(Resource.Id.spinner);
		}

		private void displayAvailableSchools(List<School> schools)
		{
			List<string> schoolsString = new List<string>();
			foreach (School school in schools)
			{
				schoolsString.Add(school.Name);
			}
			ArrayAdapter<string> schoolListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, schoolsString);

			schoolListView.Adapter = schoolListAdapter;
		}

		private void handleEvents()
		{
			schoolListView.ItemClick += SchoolListView_ItemClick;
			searchBar.QueryTextChange += SearchBar_QueryTextChange;
			//listen to items clicked in the drawer
			mDrawerList.ItemClick += DrawerListView_ItemClick;
		}

		private void SearchBar_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
		{
			ArrayAdapter<string> schoolListAdapter = (ArrayAdapter<string>)schoolListView.Adapter;
			schoolListAdapter.Filter.InvokeFilter(e.NewText);
		}

		private void SchoolListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			//get which school the user clicked on
			int clickedSchoolId = e.Position;
			//var clickedSchool = schoolListView.GetItemAtPosition(e.Position);

			//logging for test
			string tag = "MainActivity";
			Log.Info(tag, clickedSchoolId.ToString());

			//goto the search class activity providing the school id chosen and its name
			var intent = new Intent(this, typeof(SearchClassActivity));
			intent.PutExtra("selectedSchoolId", clickedSchoolId);
			StartActivity(intent);
		}
	}
}

