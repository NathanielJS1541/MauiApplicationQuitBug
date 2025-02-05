# MauiApplicationQuitBug

Basic application to reproduce a bug with `Application.Quit()` in .NET MAUI.

## Repro Steps

1. Build and deploy the app. Note that although this worked in `Debug` for me,
   since it is time-sensitive you may need to build using the `Release`
   configuration depending on your hardware.
2. Once the app launches, click the button that just says "Click to close the
   app". This button will write the current time to the Preferences API and the
   close the app using `Application.Quit()`.
3. Launch the app again. You should see that the "Last Quit Time" has not been
   updated. This indicates that the Preferences API applied the changes in
   memory, but they were never committed to disk.
4. Now click the button that says "Click to close the app with workaround".
   This button will again write the current time to the Preferences API, and
   then block using `ISharedPreferencesEditor.Commit()` until the changes are
   written to disk before calling `Application.Quit()`.
5. Launch the app again. You should now see that the "Last Quit Time" has been
   updated, indicating that the Preferences API applied the changes in memory,
   and then was able to finish committing the changes to disk.
