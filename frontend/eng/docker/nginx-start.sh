#!/bin/sh
# vim:sw=4:ts=4:et

env | while read line; do

	key=$(echo $line | cut -d= -f1)
	value=$(echo $line | cut -d= -f2)
	if [[ "var" == $(echo $key | cut -d- -f1) ]];
	then
		echo "change $key"
		sed -i "s|$key|$value|g" /usr/share/nginx/html/main*.js
	else
		echo "skip $key"
	fi
done

nginx -g 'daemon off;'