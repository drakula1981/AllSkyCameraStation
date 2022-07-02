//tabs=4
// --------------------------------------------------------------------------------
// TODO fill in this information for your driver, then remove this line!
//
// ASCOM SafetyMonitor driver for HyperRouge
//
// Description:	Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam 
//				nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam 
//				erat, sed diam voluptua. At vero eos et accusam et justo duo 
//				dolores et ea rebum. Stet clita kasd gubergren, no sea takimata 
//				sanctus est Lorem ipsum dolor sit amet.
//
// Implements:	ASCOM SafetyMonitor interface version: <To be completed by driver developer>
// Author:		(XXX) Your N. Here <your@email.here>
//
// Edit Log:
//
// Date			Who	Vers	Description
// -----------	---	-----	-------------------------------------------------------
// dd-mmm-yyyy	XXX	6.0.0	Initial edit, created from ASCOM driver template
// --------------------------------------------------------------------------------
//


// This is used to define code in the template that is specific to one class implementation
// unused code can be deleted and this definition removed.
#define SafetyMonitor

using ASCOM.Astrometry.AstroUtils;
using ASCOM.DeviceInterface;
using ASCOM.HyperRouge.Model;
using ASCOM.Utilities;
using System;
using System.Collections;
using System.Globalization;
using System.Runtime.InteropServices;

namespace ASCOM.HyperRouge {
   //
   // Your driver's DeviceID is ASCOM.HyperRouge.SafetyMonitor
   //
   // The Guid attribute sets the CLSID for ASCOM.HyperRouge.SafetyMonitor
   // The ClassInterface/None attribute prevents an empty interface called
   // _HyperRouge from being created and used as the [default] interface
   //
   // TODO Replace the not implemented exceptions with code to implement the function or
   // throw the appropriate ASCOM exception.
   //

   /// <summary>
   /// ASCOM SafetyMonitor Driver for HyperRouge.
   /// </summary>
   [Guid("df73f030-e20b-432f-8fac-17edc9ce60c9")]
   [ClassInterface(ClassInterfaceType.None)]
   public class SafetyMonitor : ISafetyMonitor {
      /// <summary>
      /// ASCOM DeviceID (COM ProgID) for this driver.
      /// The DeviceID is used by ASCOM applications to load the driver at runtime.
      /// </summary>
      internal static string driverID = "ASCOM.HyperRouge.SafetyMonitor";
      // TODO Change the descriptive string for your driver then remove this line
      /// <summary>
      /// Driver description that displays in the ASCOM Chooser.
      /// </summary>
      private static string driverDescription = "ASCOM SafetyMonitor Driver for HyperRouge's observatory.";

      internal static string traceStateProfileName = "Trace Level";
      internal static string traceStateDefault = "false";
      internal static string urlProfileName = "DatasUrl";
      internal static string urlDefault = "";

      internal static string DatasUrl;

      /// <summary>
      /// Private variable to hold the connected state
      /// </summary>
      private bool connectedState;

      /// <summary>
      /// Private variable to hold an ASCOM Utilities object
      /// </summary>
      private Util utilities;

      /// <summary>
      /// Private variable to hold an ASCOM AstroUtilities object to provide the Range method
      /// </summary>
      private AstroUtils astroUtilities;

      /// <summary>
      /// Variable to hold the trace logger object (creates a diagnostic log file with information that you specify)
      /// </summary>
      internal TraceLogger tl;

      /// <summary>
      /// Initializes a new instance of the <see cref="HyperRouge"/> class.
      /// Must be public for COM registration.
      /// </summary>
      public SafetyMonitor() {
         tl = new TraceLogger("", "HyperRouge");
         ReadProfile(); // Read device configuration from the ASCOM Profile store

         tl.LogMessage("SafetyMonitor", "Starting initialisation");

         connectedState = false; // Initialise connected to false
         utilities = new Util(); //Initialise util object
         astroUtilities = new AstroUtils(); // Initialise astro-utilities object
                                            //TODO: Implement your additional construction here

         tl.LogMessage("SafetyMonitor", "Completed initialisation");
      }


      //
      // PUBLIC COM INTERFACE ISafetyMonitor IMPLEMENTATION
      //

      #region Common properties and methods.

      /// <summary>
      /// Displays the Setup Dialog form.
      /// If the user clicks the OK button to dismiss the form, then
      /// the new settings are saved, otherwise the old values are reloaded.
      /// THIS IS THE ONLY PLACE WHERE SHOWING USER INTERFACE IS ALLOWED!
      /// </summary>
      public void SetupDialog() {
         // consider only showing the setup dialog if not connected
         // or call a different dialog if connected
         if (IsConnected)
            System.Windows.Forms.MessageBox.Show("Already connected, just press OK");

         using (SetupDialogForm F = new SetupDialogForm(tl)) {
            var result = F.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) {
               WriteProfile(); // Persist device configuration values to the ASCOM Profile store
            }
         }
      }

      public ArrayList SupportedActions {
         get {
            tl.LogMessage("SupportedActions Get", "Returning empty arraylist");
            return new ArrayList();
         }
      }

      public string Action(string actionName, string actionParameters) {
         LogMessage("", "Action {0}, parameters {1} not implemented", actionName, actionParameters);
         throw new ActionNotImplementedException("Action " + actionName + " is not implemented by this driver");
      }

      public void CommandBlind(string command, bool raw) {
         CheckConnected("CommandBlind");
         // TODO The optional CommandBlind method should either be implemented OR throw a MethodNotImplementedException
         // If implemented, CommandBlind must send the supplied command to the mount and return immediately without waiting for a response

         throw new MethodNotImplementedException("CommandBlind");
      }

      public bool CommandBool(string command, bool raw) {
         CheckConnected("CommandBool");
         // TODO The optional CommandBool method should either be implemented OR throw a MethodNotImplementedException
         // If implemented, CommandBool must send the supplied command to the mount, wait for a response and parse this to return a True or False value

         // string retString = CommandString(command, raw); // Send the command and wait for the response
         // bool retBool = XXXXXXXXXXXXX; // Parse the returned string and create a boolean True / False value
         // return retBool; // Return the boolean value to the client

         throw new MethodNotImplementedException("CommandBool");
      }

      public string CommandString(string command, bool raw) {
         CheckConnected("CommandString");
         // TODO The optional CommandString method should either be implemented OR throw a MethodNotImplementedException
         // If implemented, CommandString must send the supplied command to the mount and wait for a response before returning this to the client

         throw new MethodNotImplementedException("CommandString");
      }

      public void Dispose() {
         // Clean up the trace logger and util objects
         tl.Enabled = false;
         tl.Dispose();
         tl = null;
         utilities.Dispose();
         utilities = null;
         astroUtilities.Dispose();
         astroUtilities = null;
      }

      public bool Connected {
         get {
            LogMessage("Connected", "Get {0}", IsConnected);
            return IsConnected;
         }
         set {
            tl.LogMessage("Connected", "Set {0}", value);
            if (value == IsConnected)
               return;

            connectedState = value;
            LogMessage("Connected Set", value ? $"Connecting to URL {DatasUrl}" : $"Disconnecting from URL {DatasUrl}");
         }
      }

      public string Description {
         // TODO customise this device description
         get {
            tl.LogMessage("Description Get", driverDescription);
            return driverDescription;
         }
      }

      public string DriverInfo {
         get {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            // TODO customise this driver description
            string driverInfo = "Information about the driver itself. Version: " + String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
            tl.LogMessage("DriverInfo Get", driverInfo);
            return driverInfo;
         }
      }

      public string DriverVersion {
         get {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string driverVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
            tl.LogMessage("DriverVersion Get", driverVersion);
            return driverVersion;
         }
      }

      public short InterfaceVersion {
         // set by the driver wizard
         get {
            LogMessage("InterfaceVersion Get", "1");
            return Convert.ToInt16("1");
         }
      }

      public string Name {
         get {
            string name = "HyperRouge's Observatory Safety Monitor";
            tl.LogMessage("Name Get", name);
            return name;
         }
      }

      #endregion

      #region ISafetyMonitor Implementation
      public bool IsSafe {
         get {
            var isSafe = CurrentConditions.GetInstance(DatasUrl).IsSafe;
            LogMessage("IsSafe Get", $"{isSafe}");
            return isSafe ?? false;
         }
      }

      #endregion

      #region Private properties and methods
      // here are some useful properties and methods that can be used as required
      // to help with driver development

      #region ASCOM Registration

      // Register or unregister driver for ASCOM. This is harmless if already
      // registered or unregistered. 
      //
      /// <summary>
      /// Register or unregister the driver with the ASCOM Platform.
      /// This is harmless if the driver is already registered/unregistered.
      /// </summary>
      /// <param name="bRegister">If <c>true</c>, registers the driver, otherwise unregisters it.</param>
      private static void RegUnregASCOM(bool bRegister) {
         using (var P = new Profile()) {
            P.DeviceType = "SafetyMonitor";
            if (bRegister) {
               P.Register(driverID, driverDescription);
            } else {
               P.Unregister(driverID);
            }
         }
      }

      /// <summary>
      /// This function registers the driver with the ASCOM Chooser and
      /// is called automatically whenever this class is registered for COM Interop.
      /// </summary>
      /// <param name="t">Type of the class being registered, not used.</param>
      /// <remarks>
      /// This method typically runs in two distinct situations:
      /// <list type="numbered">
      /// <item>
      /// In Visual Studio, when the project is successfully built.
      /// For this to work correctly, the option <c>Register for COM Interop</c>
      /// must be enabled in the project settings.
      /// </item>
      /// <item>During setup, when the installer registers the assembly for COM Interop.</item>
      /// </list>
      /// This technique should mean that it is never necessary to manually register a driver with ASCOM.
      /// </remarks>
      [ComRegisterFunction]
      public static void RegisterASCOM(Type t) => RegUnregASCOM(true);

      /// <summary>
      /// This function unregisters the driver from the ASCOM Chooser and
      /// is called automatically whenever this class is unregistered from COM Interop.
      /// </summary>
      /// <param name="t">Type of the class being registered, not used.</param>
      /// <remarks>
      /// This method typically runs in two distinct situations:
      /// <list type="numbered">
      /// <item>
      /// In Visual Studio, when the project is cleaned or prior to rebuilding.
      /// For this to work correctly, the option <c>Register for COM Interop</c>
      /// must be enabled in the project settings.
      /// </item>
      /// <item>During uninstall, when the installer unregisters the assembly from COM Interop.</item>
      /// </list>
      /// This technique should mean that it is never necessary to manually unregister a driver from ASCOM.
      /// </remarks>
      [ComUnregisterFunction]
      public static void UnregisterASCOM(Type t) => RegUnregASCOM(false);

      #endregion

      /// <summary>
      /// Returns true if there is a valid connection to the driver hardware
      /// </summary>
      private bool IsConnected {
         get {
            return connectedState;
         }
      }

      /// <summary>
      /// Use this function to throw an exception if we aren't connected to the hardware
      /// </summary>
      /// <param name="message"></param>
      private void CheckConnected(string message) {
         if (!IsConnected) {
            throw new NotConnectedException(message);
         }
      }

      /// <summary>
      /// Read the device configuration from the ASCOM Profile store
      /// </summary>
      internal void ReadProfile() {
         using (Profile driverProfile = new Profile()) {
            driverProfile.DeviceType = "SafetyMonitor";
            tl.Enabled = Convert.ToBoolean(driverProfile.GetValue(driverID, traceStateProfileName, string.Empty, traceStateDefault));
            DatasUrl = driverProfile.GetValue(driverID, urlProfileName, string.Empty, urlDefault);
         }
      }

      /// <summary>
      /// Write the device configuration to the  ASCOM  Profile store
      /// </summary>
      internal void WriteProfile() {
         using (Profile driverProfile = new Profile()) {
            driverProfile.DeviceType = "SafetyMonitor";
            driverProfile.WriteValue(driverID, traceStateProfileName, tl.Enabled.ToString());
            driverProfile.WriteValue(driverID, urlProfileName, DatasUrl);
         }
      }

      /// <summary>
      /// Log helper function that takes formatted strings and arguments
      /// </summary>
      /// <param name="identifier"></param>
      /// <param name="message"></param>
      /// <param name="args"></param>
      internal void LogMessage(string identifier, string message, params object[] args) {
         var msg = string.Format(message, args);
         tl.LogMessage(identifier, msg);
      }
      #endregion
   }
}
