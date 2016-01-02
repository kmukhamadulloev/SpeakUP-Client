#!/bin/bash
cd /f/Git/SpeakUP-Client
git add .
read -p "Enter commit description: " commDesc
git commit -m "$commDesc"
git push