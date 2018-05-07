using Android.App;
using Android.Widget;
using Android.OS;

namespace XamarinSockets
{
    [Activity(Label = "XamarinSockets", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private Button buttonServidor;
        private Button buttonCliente;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            this.buttonServidor = FindViewById<Button>(Resource.Id.buttonServidor);
            this.buttonCliente = FindViewById<Button>(Resource.Id.buttonCliente);

            buttonServidor.Click += ButtonServidor_Click;
            buttonCliente.Click += ButtonCliente_Click;

        }

        private void ButtonCliente_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(Cliente));
        }

        private void ButtonServidor_Click(object sender, System.EventArgs e)
        {
            StartActivity(typeof(Servidor));
        }
    }
}

