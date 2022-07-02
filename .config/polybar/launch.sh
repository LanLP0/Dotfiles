#!/bin/bash

polybar-msg cmd quit

echo "---------------$(date '+%d/%m/%Y %H:%M:%S')---------------" | tee -a /tmp/polybar.log
polybar main 2>&1 | tee -a /tmp/polybar.log & disown
