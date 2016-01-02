#!/bin/bash
cd /f/Git/SpeakUP-Client
read -p "Enter commit description: " commDesc
git add .
git commit -m "$commDesc"
git push