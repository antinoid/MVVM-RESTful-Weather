using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ViewModelBase;

namespace ViewModel
{
    public class WeatherMainViewModel : INotifyPropertyChanged
    {
        public string Location { get; set; }

        private List<KeyValuePair<string, double>> temperatueData;

        public List<KeyValuePair<string, double>> TemperatureData
        {
            get { return temperatueData; }
            set
            {
                temperatueData = value;
                //this.OnPropertyChanged("TemperatureData");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ICommand GoCommand
        {
            get
            {
                return new RelayCommand(executeGoCommand);
            }
        }

        private async void executeGoCommand()
        {
            HttpClient client = new HttpClient();
            RootObject weather;
            if (Location == null)
            {
                Location = "Burghausen";
            }
            var json = await client.GetStringAsync(
                $"http://api.openweathermap.org/data/2.5/forecast?q={Location}&appid=2de143494c0b295cca9337e1e96b00e0");
            weather = JsonConvert.DeserializeObject<RootObject>(json);

            var weatherDataList = weather.list;
            TemperatureData = new List<KeyValuePair<string, double>>();
            int i = 0;
            foreach (var weatherData in weatherDataList)
            {
                double _temperature = weatherData.main.temp.KelvinToCelsius();
                i++;
                TemperatureData.Add(new KeyValuePair<string, double>(i.ToString(), _temperature));
            }
            OnPropertyChanged("TemperatureData");
        }


        public WeatherMainViewModel()
        {
        }
    }
}
