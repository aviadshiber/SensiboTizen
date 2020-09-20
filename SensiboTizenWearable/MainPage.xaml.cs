using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Tizen.Wearable.CircularUI.Forms;
using RestSharp;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using SensiboTizenWearable.Helpers;
using Newtonsoft.Json;
using SensiboTizenWearable.models;
using System.Net;

namespace SensiboTizenWearable
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : CirclePage
    {
       
        private string deviceID;
        private bool wasStateLoaded;
        private const string ROOT_URL = "https://home.sensibo.com/api/v2";
        public MainPage()
        {
            InitializeComponent();
            Task.Run(() => loadState());

        }
        private async Task loadState()
        {
            wasStateLoaded = false;
            await GetDeviceID();
            bool state= await GetAcState();
            UpdateButtonState(StateButton, state);
            wasStateLoaded = true;

        }

        private string BuildUrlWithApiKey(string url)
        {
            return url + "?apiKey="+ Secrets.SensiboApiKey;
        }
        private string BuildEndPoint(string endpoint)
        {
            return ROOT_URL + endpoint;
        }
        
       private async Task GetDeviceID()
        {
            var url = BuildUrlWithApiKey(BuildEndPoint("/users/me/pods"));

            using (HttpClient client = new HttpClient())
            {
                string response = await client.GetStringAsync(url);
                dynamic json = JObject.Parse(response);
                deviceID = json.result[0].id;
            }
        }
        private void UpdateButtonState(Button b, bool isOn)
        {
            if (isOn)
            {
                b.Text = "Turn OFF";
                b.BackgroundColor = Color.Red;
            }
            else
            {
                b.Text = "Turn ON";
                b.BackgroundColor = Color.Green;
            }
        }

        private async Task<bool> GetAcState()
        {
            var url = BuildUrlWithApiKey(BuildEndPoint($"/pods/{deviceID}/acStates"));
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                dynamic json = JObject.Parse(response);
                dynamic acStateJson = json.result[0].acState.on;
                return acStateJson;
            }
        }
        private void ChangeACState(BoolState state)
        {
            var client = new RestClient(BuildUrlWithApiKey(BuildEndPoint($"/pods/{deviceID}/acStates/on")));
            client.Timeout = -1;
            var request = new RestRequest(Method.PATCH);
            var content = JsonConvert.SerializeObject(state);
            request.AddHeader("Content-Type", "application/json");
            request.AddParameter("application/json", content, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
        }


        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (!wasStateLoaded)
            {
                Toast.DisplayText($"Please wait and try again");
                return;
            }
            Button b = (Button)sender;
            bool isOn =await GetAcState();
            bool newState = !isOn;
            UpdateButtonState(b, newState);
            ChangeACState(new BoolState()
            {
                newValue = newState
            });
            string status = newState ? "ON" : "OFF";
            Toast.DisplayText($"The AC is now {status}");
        }

        private void HappySlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {

        }
    }
}