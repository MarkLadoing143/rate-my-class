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
using Android.Graphics;
using Android.Text.Method;


//This stuff deals with the toolbar
using Android.Support.V7.App;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using ActionBar = Android.Support.V7.App.ActionBar;

//Drawer Layout stuff
using Android.Support.V4.Widget;
using Android.Content.PM;

namespace RateMyClass
{
    [Activity(Label = "ReviewDetailsActivity", ScreenOrientation = ScreenOrientation.Portrait)]
	public class ReviewDetailsActivity : AppCompatActivity
    {
        //private Button menuButton;
        private TextView reviewTitle;
        private TextView reviewDescription;
        private RatingBar difficultyRating;
        private RatingBar reviewRating;
        private TextView reviewRecommendation;
        private Button likeButton;
        private Button dislikeButton;
        private int selectedReviewId;
        private int selectedClassId;
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
            SetContentView(Resource.Layout.ReviewDetailsView);


			initActionBar();

            // Create your application here
            getViews();

            selectedReviewId = base.Intent.GetIntExtra("selectedReviewId", -1); // gets review id from intent
            selectedClassId = base.Intent.GetIntExtra("selectedClassId", -1); //gets class id from intent


            setViews(selectedReviewId, selectedClassId);
            handleEvents();
        }


		private void initActionBar()
		{
			//Toolbar stuff
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);

			if (toolbar != null)
			{


				//initialize the drawer
				mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout_ReviewDetails);
				mDrawerList = FindViewById<ListView>(Resource.Id.drawerListView_ReviewDetails);

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




		public void getViews()
        {
            //menuButton = FindViewById<Button>(Resource.Id.go_to_menu);
            reviewTitle = FindViewById<TextView>(Resource.Id.reviewTitle);
            reviewDescription = FindViewById<TextView>(Resource.Id.reviewDescription);
            reviewRating = FindViewById<RatingBar>(Resource.Id.reviewRating);
            difficultyRating = FindViewById<RatingBar>(Resource.Id.difficultyRating);
            reviewRecommendation = FindViewById<TextView>(Resource.Id.recommendation);
            likeButton = FindViewById<Button>(Resource.Id.likeButton);
            dislikeButton = FindViewById<Button>(Resource.Id.dislikeButton);
        }

        public async void setViews(int reviewId, int classId)
        {
            List<Review> reviews = await MainActivity.azureDataAccess.getReviews(classId.ToString());

            reviewTitle.Text = reviews[reviewId].Title;
            reviewDescription.Text = reviews[reviewId].Description;
            reviewDescription.MovementMethod = new ScrollingMovementMethod();

            setDiffStars(reviews[reviewId].Difficulty);
            setRateStars(reviews[reviewId].Rating);
            setRecommendation(reviews[reviewId].Recommended);
            setLikeButton(reviews[reviewId].ThumbsUp);
            setDislikeButton(reviews[reviewId].ThumbsDown);
        }

        public void handleEvents()
        {

            likeButton.Click += likeButton_Click;
            dislikeButton.Click += dislikeButton_Click;


			//listen to items clicked in the drawer
			mDrawerList.ItemClick += DrawerListView_ItemClick;
        }

        //NOTE: The following methods do not work and give following error:
        //Microsoft.WindowsAzure.MobileServices.MobileServiceInvalidOperationException: You must be logged in to use this application
        private async void likeButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (await MainActivity.azureDataAccess.Authenticate(this))
                {
                    List<Review> reviews = await MainActivity.azureDataAccess.getReviews(selectedClassId.ToString());
                    Review review = reviews[selectedReviewId];

                    review.ThumbsUp = review.ThumbsUp + 1;
                    await MainActivity.azureDataAccess.updateReview(review);
                    setLikeButton(reviews[selectedReviewId].ThumbsUp);

                    //disable buttons after click
                    likeButton.Clickable = false;
                    dislikeButton.Clickable = false;
                    dislikeButton.SetBackgroundColor(Color.ParseColor("#ffb3b3"));
                    likeButton.SetBackgroundColor(Color.ParseColor("#bddec8"));
                }
                else
                {
                    //user not logged in
                }
            }
            catch (Exception exc)
            {
                //handle exception
            }
        }

        private async void dislikeButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (await MainActivity.azureDataAccess.Authenticate(this))
                {
                    List<Review> reviews = await MainActivity.azureDataAccess.getReviews(selectedClassId.ToString());
                    Review review = reviews[selectedReviewId];

                    review.ThumbsDown = review.ThumbsDown + 1;
                    await MainActivity.azureDataAccess.updateReview(review);
                    setDislikeButton(reviews[selectedReviewId].ThumbsDown);

                    //disable buttons after click
                    likeButton.Clickable = false;
                    dislikeButton.Clickable = false;
                    dislikeButton.SetBackgroundColor(Color.ParseColor("#ffb3b3"));
                    likeButton.SetBackgroundColor(Color.ParseColor("#bddec8"));
                }
                else
                {
                    //user not logged in
                }
            }
            catch (Exception exc)
            {
                //handle exception
            }
        }

        private void setDiffStars(int stars)
        {
            int rating = 0;
            if(stars <= 0)
            {
                rating = stars;
            }
            reviewRating.Rating = stars;
            reviewRating.NumStars = 5;
        }

        private void setRateStars(int stars)
        {
            int rating = 0;
            if (stars <= 0)
            {
                rating = stars;
            }
            difficultyRating.Rating = stars;
            difficultyRating.NumStars = 5;
        }

        private void setRecommendation(Boolean recommended)
        {
            if (recommended == false)
            {
                reviewRecommendation.Text = "NO";
                reviewRecommendation.SetBackgroundColor(Color.ParseColor("#CF4C30"));
            }
            else if (recommended == true)
            {
                reviewRecommendation.Text = "YES";
                reviewRecommendation.SetBackgroundColor(Color.ParseColor("#27A451"));
            }
        }

        private void setLikeButton(int thumbsUp)
        {
            //thumbsUp = 1000000; //DEBUG ONLY
            if (thumbsUp == 0)
            {
                likeButton.Text = "Like (0)";
            }
            else if (thumbsUp <= 9999)
            {
                likeButton.Text = "Like (" + thumbsUp.ToString() + ")";
            }
            else if (thumbsUp > 9999 && thumbsUp <= 99999)
            {
                string twoDigits = thumbsUp.ToString().Substring(0, 2);
                likeButton.Text = "Like (" + twoDigits + "K)";
            }
            else if (thumbsUp > 99999 && thumbsUp <= 999999)
            {
                string threeDigits = thumbsUp.ToString().Substring(0, 3);
                likeButton.Text = "Like (" + threeDigits + "K)";
            }
            else if (thumbsUp > 999999 && thumbsUp <= 9999999)
            {
                string oneDigit = thumbsUp.ToString().Substring(0, 1);
                likeButton.Text = "Like (" + oneDigit + "M)";
            }
            else if (thumbsUp > 9999999 && thumbsUp <= 99999999)
            {
                string twoDigits = thumbsUp.ToString().Substring(0, 2);
                likeButton.Text = "Like (" + twoDigits + "M)";
            }
            else if (thumbsUp > 99999999 && thumbsUp <= 999999999)
            {
                string threeDigits = thumbsUp.ToString().Substring(0, 3);
                likeButton.Text = "Like (" + threeDigits + "M)";
            }
            else if (thumbsUp > 999999999 && thumbsUp <= 2147483646)
            {
                string oneDigit = thumbsUp.ToString().Substring(0, 1);
                likeButton.Text = "Like (" + oneDigit + "B)";
            }
            else if (thumbsUp > 2147483646)
            {
                likeButton.Text = "Like (2B+)";
            }
        }
        
        private void setDislikeButton(int thumbsDown)
        {
            if (thumbsDown == 0)
            {
                dislikeButton.Text = "Dislike (0)";
            }
            else if (thumbsDown <= 9999)
            {
                dislikeButton.Text = "Dislike (" + thumbsDown.ToString() + ")";
            }
            else if (thumbsDown > 9999 && thumbsDown <= 99999)
            {
                string twoDigits = thumbsDown.ToString().Substring(0, 2);
                likeButton.Text = "Dislike (" + twoDigits + "K)";
            }
            else if (thumbsDown > 99999 && thumbsDown <= 999999)
            {
                string threeDigits = thumbsDown.ToString().Substring(0, 3);
                likeButton.Text = "Like (" + threeDigits + "K)";
            }
            else if (thumbsDown > 999999 && thumbsDown <= 9999999)
            {
                string oneDigit = thumbsDown.ToString().Substring(0, 1);
                likeButton.Text = "Dislike (" + oneDigit + "M)";
            }
            else if (thumbsDown > 9999999 && thumbsDown <= 99999999)
            {
                string twoDigits = thumbsDown.ToString().Substring(0, 2);
                likeButton.Text = "Dislike (" + twoDigits + "M)";
            }
            else if (thumbsDown > 99999999 && thumbsDown <= 999999999)
            {
                string threeDigits = thumbsDown.ToString().Substring(0, 3);
                likeButton.Text = "Dislike (" + threeDigits + "M)";
            }
            else if (thumbsDown > 999999999 && thumbsDown <= 2147483646)
            {
                string oneDigit = thumbsDown.ToString().Substring(0, 1);
                likeButton.Text = "Dislike (" + oneDigit + "B)";
            }
            else if (thumbsDown > 2147483646)
            {
                likeButton.Text = "Dislike (2B+)";
            }
        }
    }
}