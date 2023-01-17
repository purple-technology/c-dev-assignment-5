This is an assignment for [C# developer job](https://www.purple-technology.com/positions/c-developer/) applicants.  
# C# programmer assignment
Implement similar deals monitoring module to the provided project. The module will monitor trading activity in real-time and will log any suspicous behavior.
## Watchdog project
The provided project is able to run multiple monitoring modules based on the configuration of the DI container. Each module has to implement the ```IMonitor``` interface.  
The application is configured to be run as a Windows service, but can run also as a console application.
### Similar deals monitoring module
Trading servers can have some restrictions, wicked users can try to bypass these restrictions by using multiple accounts with different names (of some relatives for example) and on multiple servers. We need to detect such a behavior in real-time and check similar deals.  
Similar deals are deals where:
* open time differs no more than one second,
* currency pair is the same and
* the difference in volume-to-balance ratio is no more than 5%.

If such a deals are detected log information including accounts and servers where the deals were detected.

### Application configuration
Watchdog uses ```appsettings{Environment}.json``` files for configuration.  
Configuration file contains the following parameters:
* open time delta,
* trade volume to balance ratio,
* servers to connect to (multiple servers can be provided at once).

### Implementation notes and requirements
* The module should be designed so that it can be easily maintained and tested (or it's parts)
* There can be a lot of real-time data incoming in a short period of time so it's necessary to block the server callback for the least amount of time possbile
* You should use available libraries reasonably (.net, nugget)
* In all cases only BUY/SELL deals are relevant
* Application should be able to compare data from multiple servers (cross server detection of similar deals is necessary)
* Incoming trade records has to be compared in one-to-many principle (each deal has to be compared to all relevant incomming deals based on the configured parameters)
* User balance should be requested with every processed deal as it changes in time
* Volume to balance ratio is ratio between the deal volume and current user balance  
  ```|(deal1.VolumeToBalanceRatio - deal2.VolumeToBalanceRatio)| <= configuration.VolumeToBalanceRatio```
* Deal IDs (Order) and user logins are unique only within a server
* Be aware, that ```GetUserBalance``` method within ```MT5Api``` cannot be called from event handlers (**bonus question** - do you know why?)
* The solution should be provided in the form of a pull request to this repository

### Examples
**Example 1**  
List of incoming deals:  
**Deal #1**, Balance 10 000, Buy EURUSD 1 lot at 2019-05-12 14:43:12  
**Deal #2**, Balance 10 000, Sell GBPUSD 0.2 lots at 2019-05-12 14:43:23  
**Deal #3**, Balance 10 000, Sell GBPUSD 0.21 lot at 2019-05-12 14:43:24 _<- triggered match with deal #2_

**Example 2**  
List of incoming deals:  
**Deal #1**, Balance 10 000, Buy EURUSD 1 lot at 2019-05-12 14:43:12  
**Deal #2**, Balance 10 000, Sell GBPUSD 0.2 lots at 2019-05-12 14:43:23  
**Deal #3**, Balance 1 000, Sell GBPUSD 1.2 lots at 2019-05-12 14:43:23  
**Deal #4**, Balance 10 000, Sell GBPUSD 0.21 lot at 2019-05-12 14:43:24 _<- triggered match with deal #2_  
**Deal #5**, Balance 20 000, Sell GBPUSD 0.4 lot at 2019-05-12 14:43:24 _<- triggered match with deal #2 and deal #4_  

## Project structure
### PurpleTechnology.MT5Wrapper
Serves as a wrapper above the _Metaquotes MT5 trading platform API_. What should interest you is ```IMT5ApiFactory``` to create ```IMT5Api``` instances. Implementation of this interface will be used to connect to the trading server and to receive Deals.
### PurpleTechnology.Watchdog.Config
Contains general configuration classes mapped from configuration files.
### PurpleTechnology.Watchdog.Service
Creates application service, loads configuration files and configures DI container.
### PurpleTechnology.Watchdog.DealsMonitor
This library will contain your implementation of the monitoring module.
### PurpleTechnology.Watchdog.DealsMonitor.Tests
Implement unit tests for the classes you will implement in the ```PurpleTechnology.Watchdog.DealsMonitor``` library.