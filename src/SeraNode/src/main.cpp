#include <Arduino.h>
#include <WiFi.h>
#include <HTTPClient.h>
#include <AsyncTCP.h>
#include <ESPAsyncWebServer.h>
#include "secret.h"

const int NODE_ID = 1;


const String BASE_SERVER_URL = "http://10.134.100.84:3000";

const int DATA_SEND_REPEAT_MS = 1000;

const int WIFI_TIMEOUT = 30000;

const int SOLENOID_TIMEOUT = 3000;

const int PIN_SOLENOID = 25;
const int PIN_MOISTURE_SENSORS[] = {32,33};

AsyncWebServer localServer(80);
ulong solenoidTimeout = 0;

void solenoidCheck();

enum Sound
{
  Begin,
  ConnectingToWifi,
  WiFiConnected,
  WiFiFailedToConnect
};

void PlaySound(Sound s)
{
  // TODO: Implement
}

bool WiFiInit()
{
  Serial.println("Connecting to WiFi...");
  PlaySound(Sound::ConnectingToWifi);
  
  WiFi.mode(wifi_mode_t::WIFI_MODE_STA);

  WiFi.begin(WIFI_SSID, WIFI_PW);

  WiFi.waitForConnectResult(WIFI_TIMEOUT);


  if(WiFi.status() == wl_status_t::WL_CONNECTED)
  {
    Serial.println("WiFi Connected!");
    Serial.println("IP Address: " + WiFi.localIP().toString());
    PlaySound(Sound::WiFiConnected);
    return true;
  }

  Serial.printf("Failed to connect to WiFi // Status: %d\n", (int)WiFi.status());
  PlaySound(Sound::WiFiFailedToConnect);
  return false;
}

bool LocalServerInit()
{
  Serial.println("Starting local server...");

  localServer.begin();

  localServer.on("/solenoid", HTTP_POST, [](AsyncWebServerRequest* request) {
    Serial.println("Received REQ on /solenoid");
    bool state = request->hasParam("state") && request->getParam("state")->value() == "1";

    if(state) solenoidTimeout = millis() + SOLENOID_TIMEOUT;
    else solenoidTimeout = 0;

    solenoidCheck();

    request->send(200, "text/plain", "New State: " + state ? "1" : "0");
  });
  
  Serial.println("Local server started");
  return true;
}

void setup() {
  Serial.begin(115200);
  Serial.println("App begin");

  pinMode(PIN_SOLENOID, OUTPUT);
  digitalWrite(PIN_SOLENOID, HIGH);

  if(!WiFiInit()) ESP.restart();
  if(!LocalServerInit()) ESP.restart(); 
}

int HTTPPost(const String url) {
  WiFiClient client;
  HTTPClient http;

  http.begin(client, url);
  int httpResponseCode = http.POST("{}");
  http.end();

  return httpResponseCode;
}


int GetMoistureLevel(int i)
{
  return analogRead(PIN_MOISTURE_SENSORS[i]);
}

bool SendData()
{
  Serial.println("Sending data...");
  int moistureLevelCount = sizeof(PIN_MOISTURE_SENSORS)/sizeof(PIN_MOISTURE_SENSORS[0]);

  int moistureLevel = 0;

  for(int i = 0; i<moistureLevelCount; i++)
  {
    moistureLevel += GetMoistureLevel(i);
  }

  moistureLevel /= moistureLevelCount;

  Serial.println(moistureLevel);

  String url = BASE_SERVER_URL + "/nodedata/nodedata?NodeID=" + String(NODE_ID) + "&moistureval=" + String(moistureLevel);

  int code = HTTPPost(url);

  if(code < 200 || code > 299)
  {

    Serial.printf("Failed to send data. [Status Code: %d]\n", code);
    return false;
  }

  Serial.println("Data sent successfully");
  return true;
}

void solenoidCheck()
{
  bool solState = millis() < solenoidTimeout;

  digitalWrite(PIN_SOLENOID, !solState);
}

ulong nextDataSend = 0;
void dataSendCheck()
{
  if(nextDataSend < millis())
  {
    bool suc = SendData();
    nextDataSend = millis() + DATA_SEND_REPEAT_MS;
  }
}

void loop() {

 solenoidCheck(); 
 dataSendCheck();
}