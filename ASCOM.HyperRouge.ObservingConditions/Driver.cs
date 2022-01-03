//tabs=4
// --------------------------------------------------------------------------------
// TODO fill in this information for your driver, then remove this line!
//
// ASCOM ObservingConditions driver for HyperRouge
//
// Description:	Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam 
//				nonumy eirmod tempor invidunt ut labore et dolore magna aliquyam 
//				erat, sed diam voluptua. At vero eos et accusam et justo duo 
//				dolores et ea rebum. Stet clita kasd gubergren, no sea takimata 
//				sanctus est Lorem ipsum dolor sit amet.
//
// Implements:	ASCOM ObservingConditions interface version: <To be completed by driver developer>
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
#define ObservingConditions

using ASCOM.Astrometry.AstroUtils;
using ASCOM.DeviceInterface;
using ASCOM.HyperRouge.Model;
using ASCOM.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace ASCOM.HyperRouge {
   //
   // Your driver's DeviceID is ASCOM.HyperRouge.ObservingConditions
   //
   // The Guid attribute sets the CLSID for ASCOM.HyperRouge.ObservingConditions
   // The ClassInterface/None attribute prevents an empty interface called
   // _HyperRouge from being created and used as the [default] interface
   //
   // TODO Replace the not implemented exceptions with code to implement the function or
   // throw the appropriate ASCOM exception.
   //

   /// <summary>
   /// ASCOM ObservingConditions Driver for HyperRouge.
   /// </summary>
   [Guid("e2f84c4a-8688-430c-8687-a77be3fe200d")]
   [ClassInterface(ClassInterfaceType.None)]
   public class ObservingConditions : IObservingConditions {
      /// <summary>
      /// ASCOM DeviceID (COM ProgID) for this driver.
      /// The DeviceID is used by ASCOM applications to load the driver at runtime.
      /// </summary>
      internal static string driverID = "ASCOM.HyperRouge.ObservingConditions";
      // TODO Change the descriptive string for your driver then remove this line
      /// <summary>
      /// Driver description that displays in the ASCOM Chooser.
      /// </summary>
      private static readonly string driverDescription = "ASCOM ObservingConditions Driver for HyperRouge's Observatory.";

      internal static System.Threading.Mutex mutex = new System.Threading.Mutex();
      internal static string traceStateProfileName = "Trace Level";
      internal static string traceStateDefault = "false";
      internal static string urlProfileName = "DatasUrl";
      internal static string urlDefault = "";
      internal static double AvgPer = 0.0;

      internal static bool TraceState;
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
      public ObservingConditions() {
         tl = new TraceLogger("", "HyperRouge");
         ReadProfile(); // Read device configuration from the ASCOM Profile store

         tl.LogMessage("ObservingConditions", "Starting initialisation");

         connectedState = false; // Initialise connected to false
         utilities = new Util(); //Initialise util object
         astroUtilities = new AstroUtils(); // Initialise astro-utilities object
         //TODO: Implement your additional construction here
         tl.LogMessage("ObservingConditions", "Completed initialisation");
      }


      //
      // PUBLIC COM INTERFACE IObservingConditions IMPLEMENTATION
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

      public void CommandBlind(string command, bool raw) => throw new MethodNotImplementedException("CommandBlind");

      public bool CommandBool(string Command, bool Raw = false) => throw new MethodNotImplementedException("CommandBool");

      public string CommandString(string command, bool raw) => throw new MethodNotImplementedException("CommandString");

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
            LogMessage("Connected", "Set {0}", value);
            if (value == IsConnected)
               return;

            connectedState = value;
            LogMessage("Connected Set", value ? $"Connecting to URL {DatasUrl}" : $"Disconnecting from URL {DatasUrl}");
         }
      }

      public string Description {
         // TODO customise this device description
         get {
            LogMessage("Description Get", driverDescription);
            return driverDescription;
         }
      }

      public string DriverInfo {
         get {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            // TODO customise this driver description
            string driverInfo = "Information about the driver itself. Version: " + String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
            LogMessage("DriverInfo Get", driverInfo);
            return driverInfo;
         }
      }

      public string DriverVersion {
         get {
            Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            string driverVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}", version.Major, version.Minor);
            LogMessage("DriverVersion Get", driverVersion);
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
            string name = "HyperRouge's Observatory Observing Conditions";
            LogMessage("Name Get", name);
            return name;
         }
      }

      #endregion

      #region IObservingConditions Implementation

      /// <summary>
      /// Gets and sets the time period over which observations wil be averaged
      /// </summary>
      /// <remarks>
      /// Get must be implemented, if it can't be changed it must return 0
      /// Time period (hours) over which the property values will be averaged 0.0 =
      /// current value, 0.5= average for the last 30 minutes, 1.0 = average for the
      /// last hour
      /// </remarks>
      public double AveragePeriod {
         get {
            LogMessage("AveragePeriod", AvgPer.ToString());
            return AvgPer;
         }
         set {
            if (value < 0) {
               LogMessage("Average Period", "Error - Invalid Value : " + value.ToString());
               throw new InvalidValueException("Value cannot be below 0");
            } else {
               LogMessage("AveragePeriod", "Set to " + value.ToString());
               AvgPer = value;
            }
         }
      }

      /// <summary>
      /// Amount of sky obscured by cloud
      /// </summary>
      /// <remarks>0%= clear sky, 100% = 100% cloud coverage</remarks>
      public double CloudCover {
         get {
            var cloudCover = CurrentConditions.GetInstance(DatasUrl).CloudCoverage;
            LogMessage("CloudCover", $"{cloudCover}");
            return cloudCover;
         }
      }

      /// <summary>
      /// Atmospheric dew point at the observatory in deg C
      /// </summary>
      /// <remarks>
      /// Normally optional but mandatory if <see cref=" ASCOM.DeviceInterface.IObservingConditions.Humidity"/>
      /// Is provided
      /// </remarks>
      public double DewPoint {
         get {
            var dewPoint = CurrentConditions.GetInstance(DatasUrl).DewPoint;
            LogMessage("DewPoint", $"{dewPoint}");
            return dewPoint;
         }
      }

      /// <summary>
      /// Atmospheric relative humidity at the observatory in percent
      /// </summary>
      /// <remarks>
      /// Normally optional but mandatory if <see cref="ASCOM.DeviceInterface.IObservingConditions.DewPoint"/> 
      /// Is provided
      /// </remarks>
      public double Humidity {
         get {
            var humidity = CurrentConditions.GetInstance(DatasUrl).Humidity;
            LogMessage("Humidity", $"{humidity}");
            return humidity;
         }
      }

      /// <summary>
      /// Atmospheric pressure at the observatory in hectoPascals (mB)
      /// </summary>
      /// <remarks>
      /// This must be the pressure at the observatory and not the "reduced" pressure
      /// at sea level. Please check whether your pressure sensor delivers local pressure
      /// or sea level pressure and adjust if required to observatory pressure.
      /// </remarks>
      public double Pressure {
         get {
            var pressure = CurrentConditions.GetInstance(DatasUrl).Pressure;
            LogMessage("Pressure", $"{pressure}");
            return pressure;
         }
      }

      /// <summary>
      /// Rain rate at the observatory
      /// </summary>
      /// <remarks>
      /// This property can be interpreted as 0.0 = Dry any positive nonzero value
      /// = wet.
      /// </remarks>
      public double RainRate {
         get {
            LogMessage("RainRate", "get - not implemented");
            throw new PropertyNotImplementedException("RainRate", false);
         }
      }

      /// <summary>
      /// Forces the driver to immediatley query its attached hardware to refresh sensor
      /// values
      /// </summary>
      public void Refresh() {
         LogMessage("Refresh", "get - not implemented");
         throw new MethodNotImplementedException("Refresh");
      }

      /// <summary>
      /// Provides a description of the sensor providing the requested property
      /// </summary>
      /// <param name="propertyName">Name of the property whose sensor description is required</param>
      /// <returns>The sensor description string</returns>
      /// <remarks>
      /// propertyName must be one of the sensor properties, 
      /// properties that are not implemented must throw the MethodNotImplementedException
      /// </remarks>
      public string SensorDescription(string propertyName) {
         switch (propertyName.Trim().ToLowerInvariant()) {
            case "averageperiod": {
                  return "Average period in hours, immediate values are only available";
               }

            case "cloudcover": {
                  return "Cloud cover in percentage from weather info";
               }

            case "dewpoint": {
                  return "Atmospheric dew point reported in °C.";
               }

            case "humidity": {
                  return "Atmospheric humidity (%)";
               }

            case "pressure": {
                  return "Relative atmospheric presure at the observatory (hPa)";
               }

            case "rainrate": {
                  return "Rain rate (mm / hour)";
               }

            case "skybrightness": {
                  LogMessage("SensorDescription", propertyName + " - not implemented");
                  throw new MethodNotImplementedException("SensorDescription(" + propertyName + ")");
               }

            case "skyquality": {
                  LogMessage("SensorDescription", propertyName + " - not implemented");
                  throw new MethodNotImplementedException("SensorDescription(" + propertyName + ")");
               }

            case "starfwhm": {
                  LogMessage("SensorDescription", propertyName + " - not implemented");
                  throw new MethodNotImplementedException("SensorDescription(" + propertyName + ")");
               }

            case "skytemperature": {
                  return "Sky temperature in °C";
               }

            case "temperature": {
                  return "Temperature in °C";
               }

            case "winddirection": {
                  LogMessage("SensorDescription", propertyName + " - not implemented");
                  throw new MethodNotImplementedException("SensorDescription(" + propertyName + ")");
               }

            case "windgust": {
                  LogMessage("SensorDescription", propertyName + " - not implemented");
                  throw new MethodNotImplementedException("SensorDescription(" + propertyName + ")");
               }

            case "windspeed": {
                  LogMessage("SensorDescription", propertyName + " - not implemented");
                  throw new MethodNotImplementedException("SensorDescription(" + propertyName + ")");
               }
         }
         LogMessage("SensorDescription", propertyName + " - unrecognised");
         throw new InvalidValueException("SensorDescription(" + propertyName + ")");
      }

      /// <summary>
      /// Sky brightness at the observatory
      /// </summary>
      public double SkyBrightness {
         get {
            LogMessage("SkyBrightness", "get - not implemented");
            throw new PropertyNotImplementedException("SkyBrightness", false);
         }
      }

      /// <summary>
      /// Sky quality at the observatory
      /// </summary>
      public double SkyQuality {
         get {
            LogMessage("SkyQuality", "get - not implemented");
            throw new PropertyNotImplementedException("SkyQuality", false);
         }
      }

      /// <summary>
      /// Seeing at the observatory
      /// </summary>
      public double StarFWHM {
         get {
            LogMessage("StarFWHM", "get - not implemented");
            throw new PropertyNotImplementedException("StarFWHM", false);
         }
      }

      /// <summary>
      /// Sky temperature at the observatory in deg C
      /// </summary>
      public double SkyTemperature {
         get {
            var skyTemp = CurrentConditions.GetInstance(DatasUrl).SkyTemperature;
            LogMessage("SkyTemperature", $"{skyTemp}");
            return skyTemp;
         }
      }

      /// <summary>
      /// Temperature at the observatory in deg C
      /// </summary>
      public double Temperature {
         get {
            var temperature = CurrentConditions.GetInstance(DatasUrl).Temperature;
            LogMessage("Temperature", $"{temperature}");
            return temperature;
         }
      }

      /// <summary>
      /// Provides the time since the sensor value was last updated
      /// </summary>
      /// <param name="propertyName">Name of the property whose time since last update Is required</param>
      /// <returns>Time in seconds since the last sensor update for this property</returns>
      /// <remarks>
      /// propertyName should be one of the sensor properties Or empty string to get
      /// the last update of any parameter. A negative value indicates no valid value
      /// ever received.
      /// </remarks>
      public double TimeSinceLastUpdate(string propertyName) {
         var varTimeSinceLastUpdate = (DateTime.Now - CurrentConditions.LastUpdate).TotalSeconds;
         LogMessage("TimeSinceLastUpdate", "Latest : " + varTimeSinceLastUpdate.ToString());
         switch (propertyName.Trim().ToLowerInvariant()) {
            case "": {
                  LogMessage("TimeSinceLastUpdate", "Latest : " + varTimeSinceLastUpdate.ToString());
                  return varTimeSinceLastUpdate;
               }

            case "cloudcover": {
                  LogMessage("TimeSinceLastUpdate", propertyName + " : " + varTimeSinceLastUpdate.ToString());
                  return varTimeSinceLastUpdate;
               }

            case "dewpoint": {
                  LogMessage("TimeSinceLastUpdate", propertyName + " : " + varTimeSinceLastUpdate.ToString());
                  return varTimeSinceLastUpdate;
               }

            case "humidity": {
                  LogMessage("TimeSinceLastUpdate", propertyName + " : " + varTimeSinceLastUpdate.ToString());
                  return varTimeSinceLastUpdate;
               }

            case "pressure": {
                  LogMessage("TimeSinceLastUpdate", propertyName + " : " + varTimeSinceLastUpdate.ToString());
                  return varTimeSinceLastUpdate;
               }

            case "rainrate": {
                  LogMessage("TimeSinceLastUpdate", propertyName + " : " + varTimeSinceLastUpdate.ToString());
                  return varTimeSinceLastUpdate;
               }

            case "skybrightness": {
                  LogMessage("TimeSinceLastUpdate", propertyName + " - not implemented");
                  throw new MethodNotImplementedException("TimeSinceLastUpdate(" + propertyName + ")");
               }

            case "skyquality": {
                  LogMessage("TimeSinceLastUpdate", propertyName + " - not implemented");
                  throw new MethodNotImplementedException("TimeSinceLastUpdate(" + propertyName + ")");
               }

            case "starfwhm": {
                  LogMessage("TimeSinceLastUpdate", propertyName + " - not implemented");
                  throw new MethodNotImplementedException("TimeSinceLastUpdate(" + propertyName + ")");
               }

            case "skytemperature": {
                  LogMessage("TimeSinceLastUpdate", propertyName + " : " + varTimeSinceLastUpdate.ToString());
                  return varTimeSinceLastUpdate;
               }

            case "temperature": {
                  LogMessage("TimeSinceLastUpdate", propertyName + " : " + varTimeSinceLastUpdate.ToString());
                  return varTimeSinceLastUpdate;
               }

            case "winddirection": {
                  LogMessage("TimeSinceLastUpdate", propertyName + " - not implemented");
                  throw new MethodNotImplementedException("TimeSinceLastUpdate(" + propertyName + ")");
               }

            case "windgust": {
                  LogMessage("TimeSinceLastUpdate", propertyName + " - not implemented");
                  throw new MethodNotImplementedException("TimeSinceLastUpdate(" + propertyName + ")");
               }

            case "windspeed": {
                  LogMessage("TimeSinceLastUpdate", propertyName + " - not implemented");
                  throw new MethodNotImplementedException("TimeSinceLastUpdate(" + propertyName + ")");
               }
         }
         LogMessage("TimeSinceLastUpdate", propertyName + " - unrecognised");
         throw new InvalidValueException("TimeSinceLastUpdate(" + propertyName + ")");
      }

      /// <summary>
      /// Wind direction at the observatory in degrees
      /// </summary>
      /// <remarks>
      /// 0..360.0, 360=N, 180=S, 90=E, 270=W. When there Is no wind the driver will
      /// return a value of 0 for wind direction
      /// </remarks>
      public double WindDirection {
         get {
            LogMessage("WindDirection", "get - not implemented");
            throw new PropertyNotImplementedException("WindDirection", false);
         }
      }

      /// <summary>
      /// Peak 3 second wind gust at the observatory over the last 2 minutes in m/s
      /// </summary>
      public double WindGust {
         get {
            LogMessage("WindGust", "get - not implemented");
            throw new PropertyNotImplementedException("WindGust", false);
         }
      }

      /// <summary>
      /// Wind speed at the observatory in m/s
      /// </summary>
      public double WindSpeed {
         get {
            LogMessage("WindSpeed", "get - not implemented");
            throw new PropertyNotImplementedException("WindSpeed", false);
         }
      }

      #endregion

      #region private methods

      #region calculate the gust strength as the largest wind recorded over the last two minutes

      // save the time and wind speed values
      private Dictionary<DateTime, double> winds = new Dictionary<DateTime, double>();

      private double gustStrength;

      private void UpdateGusts(double speed) {
         Dictionary<DateTime, double> newWinds = new Dictionary<DateTime, double>();
         var last = DateTime.Now - TimeSpan.FromMinutes(2);
         winds.Add(DateTime.Now, speed);
         var gust = 0.0;
         foreach (var item in winds) {
            if (item.Key > last) {
               newWinds.Add(item.Key, item.Value);
               if (item.Value > gust)
                  gust = item.Value;
            }
         }
         gustStrength = gust;
         winds = newWinds;
      }

      #endregion

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
            P.DeviceType = "ObservingConditions";
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
      private bool IsConnected => connectedState;

      /// <summary>
      /// Use this function to throw an exception if we aren't connected to the hardware
      /// </summary>
      /// <param name="message"></param>
      private void CheckConnected(string message) {
         if (!IsConnected) throw new NotConnectedException(message);
      }

      /// <summary>
      /// Read the device configuration from the ASCOM Profile store
      /// </summary>
      internal void ReadProfile() {
         using (Profile driverProfile = new Profile()) {
            driverProfile.DeviceType = "ObservingConditions";
            TraceState = Convert.ToBoolean(driverProfile.GetValue(driverID, traceStateProfileName, string.Empty, traceStateDefault));
            DatasUrl = driverProfile.GetValue(driverID, urlProfileName, string.Empty, urlDefault);
            tl.Enabled = TraceState;
         }
      }

      /// <summary>
      /// Write the device configuration to the  ASCOM  Profile store
      /// </summary>
      internal void WriteProfile() {
         using (Profile driverProfile = new Profile()) {
            driverProfile.DeviceType = "ObservingConditions";
            driverProfile.WriteValue(driverID, traceStateProfileName, TraceState.ToString());
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
