namespace BlackOut
{
    /// <summary>
    /// Interaction logic for WindowLogin.xaml
    /// </summary>
    public partial class WindowLogin
    {

        public Mode KindMode { get; set; }
        public WindowLogin()
        {
            InitializeComponent();
        }
    }

    public enum Mode
    {
        SetPassword = 0,
        Unlock = 1
    }
}
