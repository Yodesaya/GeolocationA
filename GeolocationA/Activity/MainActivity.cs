using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.V7.App;
using Android.Content;
using GeolocationA.Helper;
using System;
using Java.Lang;
using GeolocationA.Model;
using Newtonsoft.Json;

namespace GeolocationA
{
	[Activity(Label = "GeolocationA", MainLauncher = true, Icon ="@drawable/icon",Theme="@style/Theme.AppCompat.Light.NoActionBar")]
	public class MainActivity : AppCompatActivity
	{
		Button btnShowCoord;
		EditText edtAddress;
		public TextView txtCoord;

		protected override void OnCreate(Bundle bundle) //savedInstanceState
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);

			btnShowCoord = FindViewById<Button>(Resource.Id.btnShowCoordinates);
			edtAddress = FindViewById<EditText>(Resource.Id.edtAddress);
			txtCoord = FindViewById<TextView>(Resource.Id.txtCoordinates);

			btnShowCoord.Click += delegate
			{
				new GetCoordinates(this,this).Execute(edtAddress.Text.Replace(" ","+"));
			};
		}
	}

	internal class GetCoordinates:AsyncTask<string,Java.Lang.Void,string>
	{
		Context context;
		ProgressDialog dialog;
		MainActivity activity;
		public GetCoordinates(MainActivity activity,Context context)
		{
			this.context = context;
			this.activity = activity;
			dialog = new ProgressDialog(context);
		}

		protected override void OnPreExecute()
		{
			base.OnPreExecute();
			dialog.SetMessage("Please wait...");
			dialog.SetCanceledOnTouchOutside(false);
			dialog.Show();
		}

		protected override string RunInBackground(params string[] @params)
		{
			string response;
			try
			{
				string address = @params[0];
				HttpDataHandler http = new HttpDataHandler();
				string url = string.Format("https://maps.googleapis.com/maps/api/geocode/json?address={0}", address);
				response = http.GetHTTPData(url);
				return response;
			}catch(Exception ex) { return null; }
			//throw new System.NotImplementedException();
		}

		protected override void OnPostExecute(string result)
		{
			try
			{
				Geocoding dataResult = JsonConvert.DeserializeObject<Geocoding>(result);
				string lat = dataResult.results[0].geometry.location.lat.ToString();
				string lng = dataResult.results[0].geometry.location.lng.ToString();

				activity.txtCoord.Text = $"Coordinates : {lat} / {lng}";
			}catch(System.Exception ex)
			{
				
			}
			if (dialog.IsShowing)
				dialog.Dismiss();
			//base.OnPostExecute(result);
		}
	}
}

