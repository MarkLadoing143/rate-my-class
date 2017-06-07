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

namespace RateMyClass.classes
{
    public class Review
    {
        public string Id { get; set; }
        private Boolean recommended;
		private int thumbsUp;
		private int thumbsDown;
		private int difficulty;
		private int rating;
		private string title;
        private string description;
        public string classId { get; set; }
        public string userId { get; set; }

        //Will need to update this to include new properties
        public Review(string title, int rating, string description, string classId, Boolean recommended, int difficulty, string userId)
        {
            this.title = title;
            this.rating = rating;
            this.description = description;
            this.classId = classId;
            this.recommended = recommended;
            this.difficulty = difficulty;
            this.userId = userId;
        }

		public Boolean Recommended
        {
			set
			{
                recommended = value;
			}
			get
			{
				return recommended;
			}
		}

		public int ThumbsUp
		{
			get
			{
				return thumbsUp;
			}
			set
			{
				thumbsUp = value;
			}
		}

		public int ThumbsDown
		{
			get
			{
				return thumbsDown;
			}
			set
			{
				thumbsDown = value;
			}
		}

		public int Difficulty
		{
			set
			{
				difficulty = value;
			}
			get
			{
				return difficulty;
			}
		}

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

        public int Rating
        {
            get
            {
                return rating;
            }

            set
            {
                rating = value;
            }
        }

        public string Description
        {
            get
            {
                return description;
            }

            set
            {
                description = value;
            }
        }
    }
}