# Test task for Labinvent

## Terms of the task
Located in [this file](test_task(C#).pdf).

## Steps to Deploy:

0. **Install Docker Desktop:**
   - Make sure to install the appropriate version for your operating system. For Windows users, the installation link is [here](https://docs.docker.com/desktop/install/windows-install/).

1. **Configure Docker Compose File:**
   - Open the [`docker-compose.yml`](docker-compose.yml) file and locate [line 13](docker-compose.yml#L13).
   - If you don't have your own directory with .xml files, you can delete this line and uncomment the line following it with the path to the test data.
   - Otherwise replace the `FILEPATH` placeholder with the path to the folder where your XML files will be stored.
      - After the changes, it should look something like this(for Windows):
   ```yml
     volumes:
        - C:\Users\Admin\Desktop\XML\:/app/data/XMLStorage
   ```

2. **Open Command Prompt (CMD) in [Repository Root](.):**
   - It's tough, but I'm sure you'll do well.

3. **Deploy Containers:**
   - Run the following command:
     ```bash
     docker compose up -d
     ```
   This command initiates the container deployment. Be patient, as the process might take some time.

4. **Check Deployed Containers:**
   - Run the following command:
     ```bash
     docker ps --format {{.Names}}
     ```
   Copy the name of the container in which RabbitMQ is deployed.

5. **Apply Configuration to RabbitMQ:**
   - Run the following command:
     ```bash
     docker exec -d [RABBITMQ_CONTAINER_NAME] sh /usr/local/bin/init.sh
     ```
   This command configures RabbitMQ by creating the required users for the application.
     - Replace `[RABBITMQ_CONTAINER_NAME]` with the actual name of your RabbitMQ container.
     - `/usr/local/bin/init.sh` is the full path to the script inside the container.
   
      >**If you want to change script path, change the following lines**
      >- [Line 44 in docker-compose.yml](docker-compose.yml#L44)
      >- [Line 3 in Dockerfile.RabbitMQ](Docker/Dockerfile.RabbitMQ#L3)
      >- [Line 5 in Dockerfile.RabbitMQ](Docker/Dockerfile.RabbitMQ#L5)

6. **Access Database Entries:**
   - Open your browser and navigate to `localhost:8082` to explore the database entries generated by the microservice.

7. **You're Done!**
   - Congratulations! You're all set up and ready to enjoy the application.


## Changing Applications Settings

You have the flexibility to modify various application properties by editing the `appsettings.json` files for the following services:

- **FileParserService:** [appsettings.json](LabinventTestTask.FileParserService/appsettings.json)
- **DataProcessorService:** [appsettings.json](LabinventTestTask.DataProcessorService/appsettings.json)

Most of the customization options are self-describing.

#### Example Customizations:

1. Enable or Disable File Logging for the DataProcessorService:
   - In the `DataProcessorService` settings file, modify the ["IsFileLoggingEnabled" parameter in line 4](LabinventTestTask.DataProcessorService/appsettings.json#L4).

2. Configure the xml file directory check interval for the FileParserService:
   - In the `FileParserService` settings file, change the number of milliseconds in the ["ServiceTimeoutMs" parameter in line 10](LabinventTestTask.FileParserService/appsettings.json#L10).

Feel free to explore and tailor the customization options according to your needs.

## Afterword

Feel free to explore and modify the settings to suit your needs. If you have any problems or questions, please refer to the documentation or contact me for assistance.

Enjoy your delightful experience! 😊
