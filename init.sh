rabbitmqctl set_user_tags admin administrator ;

rabbitmqctl add_user FileParserService FileParserService ;
rabbitmqctl set_permissions -p / FileParserService ".*" ".*" "" ;

rabbitmqctl add_user DataProcessingService DataProcessingService ;
rabbitmqctl set_permissions -p / DataProcessingService ".*" "" ".*" ;