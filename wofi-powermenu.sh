#!/bin/bash

op=$( echo -e " Poweroff\n Reboot\n Suspend\n Lock\n Logout\n Cancel" | wofi -i --dmenu | awk '{print tolower($2)}' )

# If op is cancel, exit
if [ "${op:-cancel}" == "cancel" ]
then
    exit 0
fi
# Comfirm
op1=$( echo -e " Yes\n Cancel" | wofi -id -p "Do you want to $op?" | awk '{print tolower($2)}' )
if [ "${op1:-cancel}" == "cancel" ]
then
    exit 0
fi

case $op in 
        poweroff)
                ;&
        reboot)
                ;&
        suspend)
                systemctl $op
                ;;
        lock)
		swaylock
                ;;
        logout)
                swaymsg exit
                ;;
esac
