# AllSkyCameraStation
Together grafting on the work of Thomas Jacquin (https://github.com/thomasjacquin/allsky) to extend the capacities of an AllSky camera based on a Raspberry Pi and a camera (Zwo or the Raspberry HQ camera).
A .NET 6 service (required runtime installation explained on this page: https://docs.microsoft.com/fr-fr/dotnet/iot/deployment) checks:
- A BME280 type detector to obtain the weather information Temperature, humidity and pressure then to calculate a set of data related to these 3 information (Dew point, HeatIndex, Cloud cover height and a temperature index to calculate if or not to trigger a relay to start the heating of the dome).
- An MLX90614 sensor measures the temperature of the sky and determines, according to an algorithm inspired by this discussion: https://www.webastro.net/forums/topic/184347-fabrication-d%C3%A9tecteur-de-nuage-pas -check /, the rate of cloud cover and therefore allows to determine if the conditions are met to start an astro session.

The data collected by the sensors are stored in a LiteDB database and are also output in csv format as well as in two json files:
- current.json restores the last measurements made by the sensors
- allSkyDatas.json stores all data per 24h. Every evening at midnight, the file of the day before is archived under the name yyyyMMdd_allSkyDatas.json
- json data can be served by an independent web server or by the lighttpd server installed with the web interface of the AllSky suite.

Two ASCOM Drivers are also developed for personal needs as part of the implementation of an automated observatory:
- ASCOM.HyperRouge.ObservingConditions is an ASCOM driver using the following weather information:
  - Temperature
  - Humidity
  - Dew point
  - Atmospheric pressure
  - Cloud cover rate
  - Sky temperature
- ASCOM.HyperRouge.SafetyMonitor is an ASCOM pilot allowing to determine, according to the cloud cover, if the conditions are favorable or not to start a session

## Raspberry Pi installation INSTRUCTIONS:

- 1 - Follow the installation tutorial of the Allsky suite (basic software + Web gui)
- 2 - Install the .NET runtime 6
- 3 - Unzip the archive AllSkyCameraConditionService_1.0.zip in / usr / bin / AllSkyService
- 4 - execute the following commands:
  - cd / usr / bin / AllSkyService
  - sudo cp AllSkyConditionsService.service /etc/systemd/system/AllSkyConditionsService.service
  - sudo systemctl daemon-reload
  - sudo nano AllSkyCameraConditionService.dll.config (edition of the service configuration
  - sudo systemctl start AllSkyConditionsService
  - sudo systemctl status AllSkyConditionsService
