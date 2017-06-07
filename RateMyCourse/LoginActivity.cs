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
using Android.Gms.Auth.Api.SignIn;
using Android.Gms.Common.Apis;
using Android.Support.V4.App;
using Android.Gms.Auth.Api;
using Android.Gms.Common;
using System.Json;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json.Linq;

namespace RateMyClass
{
    [Activity(Label = "LoginActivity")]
    public class LoginActivity : FragmentActivity, GoogleApiClient.IOnConnectionFailedListener, View.IOnClickListener
    {
        private GoogleApiClient mGoogleApiClient;

        public void OnConnectionFailed(ConnectionResult result)
        {
            throw new NotImplementedException();
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Login);

            //configure sign-in to request the user's ID, email address, and basic profile. ID and basic profile are included in DefaultSignIn.
            GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DefaultSignIn)
                    .RequestEmail()
                    .RequestIdToken(GetString(Resource.String.server_client_id))
                    .Build();

            //build a GoogleApiClient with access to the Google Sign-In API and the options specified by gso.
            mGoogleApiClient = new GoogleApiClient.Builder(this)
                    .EnableAutoManage(this, this)
                    .AddApi(Auth.GOOGLE_SIGN_IN_API, gso)
                    .Build();

            FindViewById(Resource.Id.sign_in_button).Click += LoginActivity_Click;
        }

        private void LoginActivity_Click(object sender, EventArgs e)
        {
            Intent signInIntent = Auth.GoogleSignInApi.GetSignInIntent(mGoogleApiClient);
            StartActivityForResult(signInIntent, 0);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            //result returned from launching the Intent from GoogleSignInApi.getSignInIntent(...);
            if (requestCode == 0)
            {
                GoogleSignInResult result = Auth.GoogleSignInApi.GetSignInResultFromIntent(data);
                handleSignInResult(result);
            }
        }

        private void handleSignInResult(GoogleSignInResult result)
        {
            if (result.IsSuccess)
            {
                GoogleSignInAccount user = result.SignInAccount;
                FindViewById<TextView>(Resource.Id.Name).Text = user.Id; //ID USED TO IDENTIFY THE PERSON BUT SHOULD NOT BE PASSED AS IT CAN BE FAKED - THIS WILL BE SAVED IN OUR DB
                FindViewById<TextView>(Resource.Id.Name).Text = user.IdToken; //ONLY PASS ID TOKEN TO BACKEND - this is to verify that the person they claim to be is valid and logged in

                //verify user
                verifyIdToken2(user.IdToken);
            }
            else
            {
                //FindViewById<TextView>(Resource.Id.Name).Text = result.Status.StatusMessage + "---" + result.Status.StatusCode + "---" + result.Status.ToString();
            }
        }

        public void OnClick(View v)
        {
            //not needed - added an action to the button click event directly
            throw new NotImplementedException();
        }

        private async void verifyIdToken(string idToken)
        {
            string url = "https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=" + idToken;
            JsonValue json = await getJsonFromUrl(url);

            // ParseAndDisplay (json);
            FindViewById<TextView>(Resource.Id.Name).Text = json.ToString();
        }

        private async void verifyIdToken2(string idToken)
        {
            MobileServiceUser user = await MainActivity.azureDataAccess.getMobileService().LoginAsync(this, MobileServiceAuthenticationProvider.Google);

            // ParseAndDisplay (json);
            FindViewById<TextView>(Resource.Id.Name).Text = user.ToString();
        }

        // Gets weather data from the passed URL.
        private async Task<JsonValue> getJsonFromUrl(string url)
        {
            // Create an HTTP web request using the URL:
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
            request.ContentType = "application/json";
            request.Method = "GET";

            // Send the request to the server and wait for the response:
            using (WebResponse response = await request.GetResponseAsync())
            {
                // Get a stream representation of the HTTP web response:
                using (Stream stream = response.GetResponseStream())
                {
                    // Use this stream to build a JSON document object:
                    JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
                    Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

                    // Return the JSON document:
                    return jsonDoc;
                }
            }
        }
    }
}