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
using Android.Util;
using RateMyClass.classes;

//This stuff deals with the toolbar
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using ActionBar = Android.Support.V7.App.ActionBar;

//Drawer Layout stuff
using Android.Support.V4.Widget;
using Android.Content.PM;

namespace RateMyClass
{
	[Activity(Label = "SearchClassActivity", Icon = "@drawable/staricon", ScreenOrientation = ScreenOrientation.Portrait)]
	public class SearchClassActivity : AppCompatActivity
	{
		private TextView schoolNameView;
		private ListView classListView;
		private SearchView searchBar;
		private int selectedSchoolId;
		public static List<Class> classes;
		public static List<Location> locations;
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


		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			cBundle = savedInstanceState;
			SetContentView(Resource.Layout.SearchClassView);

			initActionBar();

			// Create your application here
			selectedSchoolId = base.Intent.GetIntExtra("selectedSchoolId", -1);
			getViews();
			setViews(selectedSchoolId);
			handeEvents();
		}

		private void initActionBar()
		{
			//Toolbar stuff
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

			if (toolbar != null)
			{


				//initialize the drawer
				mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout_SearchClass);
				mDrawerList = FindViewById<ListView>(Resource.Id.drawerListView_SearchClass);

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
				await MainActivity.azureDataAccess.logout();
				//refresh toolbar
				initActionBar();
				Toast.MakeText(this, "Logged Out", ToastLength.Short).Show();
			}
			else if (mDrawerListItems[clickedActionId] == "Login")
			{
				//Login user
				await MainActivity.azureDataAccess.Authenticate(this);
				//refresh toolbar
				initActionBar();
				Toast.MakeText(this, "Logged In", ToastLength.Short).Show();
			}
			else
			{
				Toast.MakeText(this, "Unknown Option Selected", ToastLength.Short).Show();
			}
		}

		private void getViews()
		{
			schoolNameView = FindViewById<TextView>(Resource.Id.schoolName);
			classListView = FindViewById<ListView>(Resource.Id.classListView);
			searchBar = FindViewById<SearchView>(Resource.Id.searchView1);
		}

		private async void setViews(int schoolId)
		{
            MainActivity.scoolId2 = schoolId;
			schoolNameView.Text = MainActivity.schools[schoolId].Name;

			//retreive classes from DB and display them
			classes = await MainActivity.azureDataAccess.getClasses(schoolId);
			locations = await MainActivity.azureDataAccess.getLocations(MainActivity.schools[schoolId].Location);
			//convert list of classes to a list of strings (class name) to display using the simple list item
			List<string> classesString = new List<string>();
			foreach (Class _class in classes)
			{
				classesString.Add(_class.name);
			}
			ArrayAdapter<string> classListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, classesString);
			classListView.Adapter = classListAdapter;
		}

		private void handeEvents()
		{
			classListView.ItemClick += ClassListView_ItemClick;
			searchBar.QueryTextChange += SearchBar_QueryTextChange;
			//listen to items clicked in the drawer
			mDrawerList.ItemClick += DrawerListView_ItemClick;
		}

		//runs any time text is change in the search field
		private void SearchBar_QueryTextChange(object sender, SearchView.QueryTextChangeEventArgs e)
		{
			ArrayAdapter<string> classListAdapter = (ArrayAdapter<string>)classListView.Adapter;
			classListAdapter.Filter.InvokeFilter(e.NewText);
		}

		//runs when the user clicks an item in the list
		private void ClassListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			//get which class the user clicked on
			int clickedClassId = e.Position;

			//open the class view activity providing the ID
			var intent = new Intent(this, typeof(ClassViewActivity));
			Bundle extra = new Bundle();
			extra.PutInt("selectedClassId", clickedClassId);
			extra.PutString("selectedLocation", MainActivity.schools[selectedSchoolId].Location);
			extra.PutInt("selectedSchoolId", selectedSchoolId);
			intent.PutExtras(extra);
			StartActivity(intent);
		}
	}
}