using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PriceGrabber.CustomControls
{
    public class WebviewContainer : ContentView
    {
        public WebviewContainer()
        {

        }

        public WebViewer WebViewer { get; set; }

        public string Title { get => WebViewer.Title; set => WebViewer.Title = value; }

        public event Action OnReturn;

    }
}
