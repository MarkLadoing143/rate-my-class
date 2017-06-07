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
using RateMyClass.classes;
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
	[Activity(Label = "ClassViewActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class ClassViewActivity : AppCompatActivity
	{
		private TextView className;
		private TextView schoolName;
		private TextView schoolLocation;
		private TextView classDescription;
		private Button addReviewButton;
		private Button addNotesButton;
		private ListView reviewList;
		private int classID;
		private string locationID;
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

			SetContentView(Resource.Layout.ClassView);

			//initialize action bar
			initActionBar();

			// Create your application here
			getViews();

			//Handle intents
			// Intent intent = base.Intent.get;
			//Bundle extras = intent.getExtras();

			classID = base.Intent.GetIntExtra("selectedClassId", -1);
			locationID = base.Intent.GetStringExtra("selectedLocation");
			setViews(classID);
			handleEvents();

		}



		private void initActionBar()
		{
			//Toolbar stuff
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

			if (toolbar != null)
			{


				//initialize the drawer
				mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout_ClassView);
				mDrawerList = FindViewById<ListView>(Resource.Id.drawerListView_ClassView);

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
			className = FindViewById<TextView>(Resource.Id.className);
			schoolName = FindViewById<TextView>(Resource.Id.schoolName);
			schoolLocation = FindViewById<TextView>(Resource.Id.schoolLocation);
			classDescription = FindViewById<TextView>(Resource.Id.classDescription);
			addReviewButton = FindViewById<Button>(Resource.Id.addReviewButton);
			reviewList = FindViewById<ListView>(Resource.Id.reviewList);
		}

		private async void setViews(int selectedClassId)
		{
			//set all the text views for the selected class

			//Set the title page
			className.Text = SearchClassActivity.classes[selectedClassId].name;

			//Set the school name
			schoolName.Text = MainActivity.schools[SearchClassActivity.classes[selectedClassId].schoolId].Name;


			//int schoolLocation =Int32.Parse(MainActivity.schools[SearchClassActivity.classes[selectedClassId].schoolId].Location);

			//Set the school location
			schoolName.Text = MainActivity.schools[SearchClassActivity.classes[selectedClassId].schoolId].Name;

			schoolLocation.Text = SearchClassActivity.locations[0].cityName + ", " + SearchClassActivity.locations[0].provinceName;


			classDescription.Text = SearchClassActivity.classes[selectedClassId].description;

			//Set the review last 
			await loadReviews(selectedClassId);
		}

		private async Task loadReviews(int selectedClassId)
        {
            List<Class> classes = await MainActivity.azureDataAccess.getClasses(MainActivity.scoolId2);
            Class chosenClass = classes[selectedClassId];
            List<Review> reviews = await MainActivity.azureDataAccess.getReviews(chosenClass.id);

            ReviewAdapter adapter = new ReviewAdapter(this, reviews);
			reviewList.Adapter = adapter;
		}

		private void handleEvents()
		{
			reviewList.ItemClick += ReviewList_ItemClick;
			addReviewButton.Click += AddReviewButton_Click;
			//listen to items clicked in the drawer
			mDrawerList.ItemClick += DrawerListView_ItemClick;
		}

		private void AddReviewButton_Click(object sender, EventArgs e)
		{
			var intent = new Intent(this, typeof(AddReviewActivity));
			intent.PutExtra("classID", classID);
			intent.PutExtra("selectedClassId", classID);
			intent.PutExtra("selectedSchoolId", base.Intent.GetIntExtra("selectedSchoolId", -1));
			StartActivityForResult(intent, AddReviewActivity.REQUEST_CODE);
		}

		private void ReviewList_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
		{
			//get which review the user clicked on
			int clickedReviewId = e.Position;

			//open the review details activity providing the ID
			var intent = new Intent(this, typeof(ReviewDetailsActivity));
			intent.PutExtra("selectedReviewId", clickedReviewId);
			intent.PutExtra("selectedClassId", classID);
			intent.PutExtra("selectedSchoolId", base.Intent.GetIntExtra("selectedSchoolId", -1));
			StartActivity(intent);
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			if (requestCode == AddReviewActivity.REQUEST_CODE)
			{
				if (resultCode == Result.Ok)
				{
					//add review is successfull - reload reviews
					loadReviews(classID);
				}
			}
		}
	}
}