# AllSkyCameraStation
Ensemble se greffant sur le travail de Thomas Jacquin (https://github.com/thomasjacquin/allsky) pour étendre les capacités d'une camera AllSky basée sur un Raspberry Pi et une camera (Zwo ou la camera Raspberry HQ).

Un service en .NET 6 (installation requise du runtime expliquée sur cette page : https://docs.microsoft.com/fr-fr/dotnet/iot/deployment) contrôle :
- Un détecteur de type BME280 pour obtenir les informations méteo Température, humidité et pression puis de calculer un ensemble de données liées à ces 3 informations (Point de rosée, HeatIndex, Hauteur de la couverture nuageuse et un index de température permettant de calculer s'il faut ou non declencher un relais pour lancer le chauffage du dome).
- Un capteur MLX90614 mesure la température du ciel et détermine, d'après un algorithme inspiré par cette discussion : https://www.webastro.net/forums/topic/184347-fabrication-d%C3%A9tecteur-de-nuage-pas-cher/, le taux de couverture nuageuse et donc permet de déterminer si les conditions sont réunies pour démarrer une session d'astro.
- Les données receuilies par les capteurs sont stockées dans une base de données LiteDB et sont également sorties au format csv ainsi que dans deux fichiers json :
  - current.json restitue les dernieres mesures effectuées par les capteurs
  - allSkyDatas.json stocke toutes les données par 24h. Tous les soirs à minuit, le fichier de la veille est archivé sous le nom aaaaMMdd_allSkyDatas.json
- les données json peuvent être servies par un serveur web indépendant ou bien par le serveur lighttpd installé avec l'interface web de la suite AllSky 

Deux Drivers ASCOM sont également développés pour des besoins personnels dans le cadre de la mise en place d'un observatoire automatisé :
- ASCOM.HyperRouge.ObservingConditions est un pilote ASCOM exploitant les informations méteo suivantes : 
  - Température
  - Humidité
  - Point de rosée
  - Pression atmosphérique
  - Taux de couverture nuageuse
  - Température du ciel
- ASCOM.HyperRouge.SafetyMonitor est un pilote ASCOM permettant de déterminer, en fonction de la couverture nuageuse, si les conditions sont propices ou non à démarrer une session 

## INSTRUCTIONS d'installation sur raspberry Pi :
 - 1 - Suivre le tutoriel d'installation de la suite Allsky (soft de base + Web gui)
 - 2 - Installer le runtime .NET 6
	- wget https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh
	- chmod +x ./dotnet-install.sh
	- ./dotnet-install.sh
 - 3 - Dezipper l'archive AllSkyCameraConditionService_1.0.zip dans /usr/bin/AllSkyService
 - 4 - executer les commandes suivantes :
	- sudo raspi-config -> Activer I2C dans les paramètres de périphériques
    - cd /usr/bin/AllSkyService
    - sudo cp AllSkyConditionsService.service /etc/systemd/system/AllSkyConditionsService.service
    - sudo systemctl daemon-reload
    - sudo nano AllSkyCameraConditionService.dll.config (edition de la configuration du service)
    - sudo systemctl start AllSkyConditionsService
    - sudo systemctl status AllSkyConditionsService
