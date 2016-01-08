using Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ViewModel
{
    public class WeatherViewModel : ViewModelBase
    {
        string spw = "j6Fwhs";
        public int GraphHeight { get; set; }
        public int GraphWidth { get; set; }

        private string _location;

        public string Location
        {
            get { return _location; }
            set { SetValue(ref _location, value); }
        }

        private string[] colors =
            {
            "#4C45B3", "#6766FA", "#46A2F3",
            "#42BBD1", "#2E9786", "#35A125",
            "#8AC749", "#F7C520", "#F69722",
            "#F44F32"
        };

        private ObservableCollection<Line> _lines;

        public ObservableCollection<Line> Lines
        {
            get { return _lines; }
            set { SetValue(ref _lines, value); }
        }

        private ICommand _goCommand;

        public ICommand GoCommand
        {
            get
            {
                if (_goCommand == null)
                {
                    _goCommand = new RelayCommand(_ => ExecuteGoCommand());
                }
                return _goCommand;
            }
            set { _goCommand = value; }
        }

        private async void ExecuteGoCommand()
        {
            string pa = "Pa";
            if (Location == null)
            {
                Location = "Gendorf,de";
            }

            String proxyURI = string.Format("{0}:{1}", "proxy.intranet.bit-gendorf.de", "8080");

            NetworkCredential proxyCreds = new NetworkCredential(
                "intranet\\kothieringer",
                spw + pa);

            WebProxy proxy = new WebProxy(proxyURI, false)
            {
                UseDefaultCredentials = false,
                Credentials = proxyCreds,
            };

            HttpClientHandler httpClientHandler = new HttpClientHandler
            {
                Proxy = proxy,
                PreAuthenticate = true,
                UseDefaultCredentials = false,
            };

            httpClientHandler.Credentials = new NetworkCredential("intranet\\kothieringer", spw + pa);

            using (HttpClient client = new HttpClient(httpClientHandler))
            {
                var json = await client.GetStringAsync(
                     $"http://api.openweathermap.org/data/2.5/forecast?q={Location}&appid=2de143494c0b295cca9337e1e96b00e0");
                var weather = JsonConvert.DeserializeObject<RootObject>(json);
                List<double> temperatureList = new List<double>();
                foreach (var item in weather.list)
                {
                    double temperature = item.main.temp.KelvinToCelsius();
                    temperatureList.Add(temperature);
                }
                createLines(temperatureList);
            }
        }

        public WeatherViewModel()
        {
            GraphHeight = 200;
            GraphWidth = 400;

            Lines = new ObservableCollection<Line>();
        }

        private void createLines(List<double> values)
        {
            Lines.Clear();
            int lineCount = values.Count - 1;
            int xAxisStep = GraphWidth / lineCount;
            int yAxisRatio = GraphHeight / 100;
            int yAxisOffset = 50;

            for (int i = 0; i < lineCount; i++)
            {
                double t1 = values[i];
                double t2 = values[i + 1];
                Point from = new Point(i * xAxisStep, GraphHeight - (yAxisRatio * (t1 + yAxisOffset)));
                Point to = new Point((i + 1) * xAxisStep, GraphHeight - (yAxisRatio * (t2 + yAxisOffset)));
                double avgTemperature = (t1 + t2) / 2;
                string color = colors[(int)(avgTemperature + 50) / colors.Length];
                Lines.Add(new Line(from, to, color));
            }
        }
    }
}
