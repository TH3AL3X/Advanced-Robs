# Advanced-Robs Wiki

Advanced theft system with included UI, works with Uconomy and AdvancedZones

## Permissions
* Police permissions: PolicePermission = **"policia"**; (This is the permission that must be added to the police, due to the number of police in order to make a robbery.)
* **advancedrobs** (This is the permission for normal users to steal defaults)

## Configuration
To use this plugin requires the Uconomy, Advanced Zones, and a workshop UI mod
* Workshop UI: [Rob UI]: https://steamcommunity.com/sharedfiles/filedetails/?id=1765561076
* Workshop UI2: [Reward UI]: https://steamcommunity.com/sharedfiles/filedetails/?id=1768031090

## Configuration AdvancedZones
* MaxPolice **(Max police for rob this theft)**
* Reward **(Reward, if the type of rob is money)**
* Time **(Time for get the reward after theft)**
* Cooldown **(Cooldown for rob again)**
* TitletUI **(Title UI of the rob)**
* TextUI **(Text UI of the rob)**
* TypeRob = "items or money" **(Type of rob, if you want to give items or experience)**
** Here you can add the items you want, and you can duplicate it**
* *items
* *items

## Installation
* Create the zone with all nodes **(/zone add "Name Rob")**
* Create all nodes **(/zone add node "Name Rob")**, make a rectangle
* Add parameters to the rob **(/addrob "Name Rob")**
* Add flag to the zone **(/zone add flag "Name Rob" robs)**
* For edit parameters go to the "AdvancedZones Directory, there you have all your stuff for edit the rob"
Then the configuration of the messages is in the plugin directory

# Configuration Example
            *UconomyOrXp = false; **(Uconomy == true, No uconomy == false)**
            *PolicePermission = "policia"; **(Police permission)**
            *UIReward = 16999; **(Reward UI ID)**
            *LeaveRobSecondsForGoBack = 1; **(Seconds for go back to the rob after you leave)**
            *startrobmessageicon = "https://lineex.es/wp-content/uploads/2018/06/alert-icon-red-11-1.png"; **(Icon Message)**
            *finishrobmessageicon = "https://lineex.es/wp-content/uploads/2018/06/alert-icon-red-11-1.png"; **(Icon Message)**
            *leavearearobmessageicon = "https://lineex.es/wp-content/uploads/2018/06/alert-icon-red-11-1.png"; **(Icon Message)**
            *iconwarn = "https://tuc.vampyrium.com/warn/triangle_1024.png"; **(Icon Message)**
            *iconwarnwithoutui = "https://tuc.vampyrium.com/warn/triangle_1024.png"; **(Icon Message)**
            
# Translations Example
             *{"startrob", "(color=red)[AdvancedRobs] {0} IS TRYING TO ROB THE {1}(/color)"}, **({0} is the name of the player who is trying to rob, {1} rob name)**
             *{"finishrob", "(color=red)[AdvancedRobs] {0} HAS STOLEN CORRECTLY THE {1} THEFT(/color)"},
             *{"leaverob", "(color=red)[AdvancedRobs] {0} IS OUT OF THE {1} THEFT AREA(/color)"},
             *{"CooldownRob", "(color=yellow)[AdvancedRobs](/color) (color=red)Cooldown for rob again {0}(/color)"},
             *{"NoPermissions", "[AdvancedRobs] You don't have permissions to rob this"},
             *{"NoPolices", "(color=yellow)[AdvancedRobs](/color) (color=red)To be able to theft, there must be a maximum of {0} polices(/color)"},
             *{"EnterPolice", "(color=yellow)[AdvancedRobs](/color) (color=red)You're a cop, you just got into the robbery(/color)"}
