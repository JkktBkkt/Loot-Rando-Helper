global using System.Windows.Forms;
global using System.Drawing;

namespace BL2_Test_Loot_Rando
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Older style of customization options for support of other platforms
            // TODO: Replace with #if ?
            // Didn't work with #if WINDOWS because winforms still "target" windows
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main_Form());
            //Application.Run(new Form1());
        }
    }
}