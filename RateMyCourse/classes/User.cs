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
    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
		public string LastName { get; set; }
    }
}