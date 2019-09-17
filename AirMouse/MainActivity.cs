using System.Drawing;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Library;
using Xamarin.Essentials;

namespace AirMouse
{
    [Activity(Label = "@string/app_name", Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static int Sensitivity;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            InitializeComponent_HomePage();
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            try
            {
                AccelerometerStop();
                NetManager.Send(NetManager.Command.ConnectClose, "");
                TcpManager.Disconnect();
            }
            catch
            {
                // ignored
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void InitializeComponent_HomePage()
        {
            SetContentView(Resource.Layout.HomePage);

            var buttonTcp = FindViewById<Button>(Resource.Id.buttonTcp);
            buttonTcp.Click += delegate { InitializeComponent_TcpPage(); };

            var buttonInfo = FindViewById<Button>(Resource.Id.buttonInfo);
            buttonInfo.Click += delegate { InitializeComponent_InfoPage(); };
        }

        private void InitializeComponent_TcpPage()
        {
            SetContentView(Resource.Layout.TcpPage);

            var editTextTcpIp = FindViewById<EditText>(Resource.Id.editTextTcpIp);

            var buttonTcpConnect = FindViewById<Button>(Resource.Id.buttonTcpConnect);
            buttonTcpConnect.Click += delegate { ButtonTcpConnectClicked(editTextTcpIp.Text); };
        }

        private void InitializeComponent_InfoPage()
        {
            SetContentView(Resource.Layout.InfoPage);
            var buttonBluetoothContinue = FindViewById<Button>(Resource.Id.buttonInfoOK);
            buttonBluetoothContinue.Click += delegate { InitializeComponent_HomePage(); };
        }

        private void InitializeComponent_MousePage()
        {
            SetContentView(Resource.Layout.MousePage);

            var seekBarSensitivity = FindViewById<SeekBar>(Resource.Id.seekBar);
            seekBarSensitivity.ProgressChanged += (sender, e) => { Sensitivity = e.Progress; };

            var buttonLeft = FindViewById<ImageButton>(Resource.Id.mouseButtonLeft);
            buttonLeft.Touch += (sender, e) =>
            {
                switch (e.Event.Action)
                {
                    case MotionEventActions.Up:
                        NetManager.Send(NetManager.Command.LeftUp, "");
                        break;
                    case MotionEventActions.Down:
                        NetManager.Send(NetManager.Command.LeftDown, "");
                        break;
                }
            };

            var buttonRight = FindViewById<ImageButton>(Resource.Id.mouseButtonRight);
            buttonRight.Touch += (sender, e) =>
            {
                switch (e.Event.Action)
                {
                    case MotionEventActions.Up:
                        NetManager.Send(NetManager.Command.RightUp, "");
                        break;
                    case MotionEventActions.Down:
                        NetManager.Send(NetManager.Command.RightDown, "");
                        break;
                }
            };

            var buttonExit = FindViewById<ImageButton>(Resource.Id.mouseButtonExit);
            buttonExit.Click += delegate
            {
                AccelerometerStop();
                NetManager.Send(NetManager.Command.ConnectClose, "");
                Sensitivity = 0;
                TcpManager.Disconnect();
                InitializeComponent_HomePage();
            };
        }

        private void ButtonTcpConnectClicked(string address)
        {
            TcpManager.Connect(address);
            if (!TcpManager.Connected) return;
            NetManager.Stream = TcpManager.GetStream();
            NetManager.Client = TcpManager.GetClient();

            InitializeComponent_MousePage();
            AccelerometerStart(SensorSpeed.Fastest);
        }

        private void Accelerometer_ReadingChange(object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading;
            var pos = new Point((int)(-data.Acceleration.X * Sensitivity), (int)(-data.Acceleration.Y * Sensitivity));
            if (pos == new Point(0, 0)) return;
            NetManager.Send(NetManager.Command.Move, $"{pos.X}|{pos.Y}");
        }

        public void AccelerometerStart(SensorSpeed speed)
        {
            if (Accelerometer.IsMonitoring) return;

            Accelerometer.ReadingChanged += Accelerometer_ReadingChange;
            Accelerometer.Start(speed);
        }

        public void AccelerometerStop()
        {
            if (!Accelerometer.IsMonitoring) return;

            Accelerometer.ReadingChanged -= Accelerometer_ReadingChange;
            Accelerometer.Stop();
        }
    }
}