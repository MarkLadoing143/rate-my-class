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
using Java.Lang;

namespace RateMyClass.classes
{
    class ReviewAdapter : BaseAdapter<Review>
    {
        private List<Review> reviews;
        private Context context;

        public ReviewAdapter(Context providedContext, List<Review> providedReviews)
        {
            this.context = providedContext;
            this.reviews = providedReviews;
        }

        public override int Count
        {
            get { return reviews.Count;}
        }

        public override Review this[int position]
        {
            get
            {
                return reviews[position];
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            throw new NotImplementedException();
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;
            if(row == null)
            {
                row = LayoutInflater.From(context).Inflate(Resource.Layout.reviewListItem, null, false);
            }

            TextView reviewTitle = row.FindViewById<TextView>(Resource.Id.title);
            TextView reviewRating = row.FindViewById<TextView>(Resource.Id.rating);
            TextView reviewDescription = row.FindViewById<TextView>(Resource.Id.description);

            reviewTitle.Text = reviews[position].Title;
			reviewRating.Text = reviews[position].Rating.ToString();
            reviewDescription.Text = reviews[position].Description;

            return row;
        }
    }
}