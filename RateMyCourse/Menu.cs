
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

namespace RateMyClass
{
	[Activity(Label = "Menu")]
	public class Menu : Activity
	{
		private Button schoolButton;
		private Button classButton;
		private Button reviewButton;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			SetContentView(Resource.Layout.Menu);

			findViews();

			handleEvents();

		}

		private void findViews()
		{
			schoolButton = FindViewById<Button>(Resource.Id.select_school);
			classButton = FindViewById<Button>(Resource.Id.select_class);
			reviewButton = FindViewById<Button>(Resource.Id.select_review);

		}


		private void handleEvents()
		{
			schoolButton.Click += delegate { StartActivity(new Intent(this, typeof(MainActivity))); };
			classButton.Click += delegate { StartActivity(new Intent(this, typeof(SearchClassActivity))); };
			reviewButton.Click += delegate { StartActivity(new Intent(this, typeof(ClassViewActivity))); };
		}
	}
}
