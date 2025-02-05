using Android.Preferences;

namespace MauiApplicationQuitBug
{
    public partial class MainPage : ContentPage
    {
        #region Constants

        #region Private

        /// <summary>
        /// The key to save the preference with for demonstration.
        /// </summary>
        private const string LastQuitTimeKey = "LastQuitTime";

        #endregion

        #endregion

        #region Construction

        public MainPage()
        {
            InitializeComponent();

            // When the page loads, display the previous value stored using the Preferences API.
            // This will fall back to "N/A" to indicate no preference has been set.
            LastQuitTime.Text = Preferences.Get(LastQuitTimeKey, "N/A");
        }

        #endregion

        #region Methods

        #region Event Handlers

        /// <summary>
        /// Handler for the "Quit" button being clicked. This attempts to write the current time as
        /// a string to the <see cref="Preferences"/> API, and then quits the app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnQuitClicked(object sender, EventArgs e)
        {
            // Save the current time as a string using the preferences API.
            Preferences.Set(LastQuitTimeKey, DateTime.Now.ToString("HH:mm:ss"));

            // Close the application.
            Application.Current?.Quit();
        }

        /// <summary>
        /// Handler for the "QuitWithWorkaround" button being clicked. This attempts to write the
        /// current time as a string to the <see cref="Preferences"/> API, manually blocks until
        /// the value is committed using a workaround, and then quits the app.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnQuitWorkaroundClicked(object sender, EventArgs e)
        {
            // Save the current time as a string using the preferences API.
            Preferences.Set(LastQuitTimeKey, DateTime.Now.ToString("HH:mm:ss"));

            // HACK! Manually block until all changes are committed to disk.

            // Get an ISharedPreferences instance, and open the ISharedPreferencesEditor for it.
#pragma warning disable CA1422
            using var sharedPreferences = PreferenceManager.GetDefaultSharedPreferences(Android.App.Application.Context);
#pragma warning restore CA1422
            using var editor = sharedPreferences?.Edit();

            // According to
            // https://developer.android.com/reference/android/content/SharedPreferences.Editor#apply():
            // > If another editor on this SharedPreferences does a regular commit() while a
            // > apply() is still outstanding, the commit() will block until all async commits
            // > are completed as well as the commit itself.
            //
            // Use this fact to block here until all outstanding applied changes are written to the
            // disk.
            editor?.Commit();

            // Close the application.
            Application.Current?.Quit();
        }

        #endregion

        #endregion
    }

}
