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

//This stuff deals with the toolbar
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using ActionBar = Android.Support.V7.App.ActionBar;

//Drawer Layout stuff
using Android.Support.V4.Widget;
using Android.Content.PM;

namespace RateMyClass
{
    [Activity(Label = "AddReviewActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class AddReviewActivity : AppCompatActivity
    {
        private int classID;
        private TextView className;
        private EditText reviewTitle;
        private EditText reviewDescription;
        private RatingBar ratingBar;
        private RatingBar ratingBarDifficulty;
        private Switch recommendedSwitch;
        private Button submitButton;
        private Boolean recommended;
        public const int REQUEST_CODE = 100;
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


			//any preprocessing before displaying main app screen put here
			azureDataAccess = new AzureDataAccess();
			azureDataAccess.loadUserFromCache();

            SetContentView(Resource.Layout.AddReviewView);


			initActionBar();

            getViews();
            classID = base.Intent.GetIntExtra("classID", -1);
            setViews(classID);
            handleEvents();
            authenticateUser();
        }

		private void initActionBar()
		{
			//Toolbar stuff
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

			if (toolbar != null)
			{


				//initialize the drawer
				mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout_AddReview);
				mDrawerList = FindViewById<ListView>(Resource.Id.drawerListView_AddReview);

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
				Intent intent = new Intent(this, typeof(ClassViewActivity));
				intent.PutExtras(bundle);
				SetResult(Result.Canceled, intent);
				Finish();
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

		private async void authenticateUser()
        {
            try
            {
                if (await MainActivity.azureDataAccess.Authenticate(this))
                {
                    //successfully authenticated
                }
                else
                {
                    Finish();
                }
            }
            catch(Exception exc)
            {
                Finish();
            }
        }

        private void getViews()
        {
            className = FindViewById<TextView>(Resource.Id.className);
            reviewTitle = FindViewById<EditText>(Resource.Id.reviewTitle);
            reviewDescription = FindViewById<EditText>(Resource.Id.reviewDescription);
            ratingBar = FindViewById<RatingBar>(Resource.Id.ratingBar);
            ratingBarDifficulty = FindViewById<RatingBar>(Resource.Id.ratingBarDifficulty);
            recommendedSwitch = FindViewById<Switch>(Resource.Id.recommendedSwitch);
            submitButton = FindViewById<Button>(Resource.Id.submitButton);
        }

        private void setViews(int classID)
        {
            className.Text = SearchClassActivity.classes[classID].name;
        }

        private void handleEvents()
        {
            recommendedSwitch.CheckedChange += Recommended_CheckedChange;
            submitButton.Click += SubmitButton_Click;
			//listen to items clicked in the drawer
			mDrawerList.ItemClick += DrawerListView_ItemClick;
        }

        private void Recommended_CheckedChange(object sender, CompoundButton.CheckedChangeEventArgs e)
        {
            recommended = e.IsChecked;
        }

        private void recommendedSwitch_Action(CompoundButton.IOnCheckedChangeListener obj)
        {
            throw new NotImplementedException();
        }

        private async void SubmitButton_Click(object sender, EventArgs e)
        {
            List<Class> classes = await MainActivity.azureDataAccess.getClasses(MainActivity.scoolId2);
            Class chosenClass = classes[classID];

            Review review = new Review(reviewTitle.Text, (int)ratingBar.Rating, reviewDescription.Text, chosenClass.id, recommended, (int)ratingBarDifficulty.Rating, "TEMPUSERID");
            await MainActivity.azureDataAccess.addReview(review);

            SetResult(Result.Ok);
            Finish();
        }
    }
}