# GPTOrganizer

GPTOrganizer is a powerful tool that leverages OpenAI's GPT-4 to automatically categorize and organize files in a directory based on their names and contents.

Disclaimer: This tool is sending file names to the open AI model but it is not reading content of the files.


## Table of Contents

- [Features](#features)
- [Getting Started](#getting-started)
  - [Cloning the Repository](#cloning-the-repository)
  - [Building the Tool](#building-the-tool)
- [Downloading Pre-built Binaries](#downloading-pre-built-binaries)
- [Usage](#usage)
- [Options](#options)

## Features

- Automatic file categorization using OpenAI.
- Customizable category definitions.
- Option to either move or copy organized files.
- Extensive logging for easy debugging.

## Getting Started

### Cloning the Repository

To clone the GPTOrganizer repository, run the following command:

```bash
git clone https://github.com/lucekp/GPTOrganizer.git
```

### Building the Tool

After cloning the repository:

1. Navigate to the project root:

    ```bash
    cd GPTOrganizer
    ```

2. Build the project using .NET:

    ```bash
    dotnet build --configuration Release
    ```

This will produce a `Release` build of the tool.

## Downloading Pre-built Binaries

If you prefer to use GPTOrganizer without building from source, you can download the pre-built binaries directly from the **Releases** section of this repository.

### Steps:

1. Navigate to the [Releases](https://github.com/lucekp/GPTOrganizer/releases) tab on the GitHub repository.
   
2. Choose the version you'd like to download and click on it.
   
3. Under **Assets**, download the appropriate binary for your operating system (e.g., `GPTOrganizer_vX.X.X_windows_x64.zip` for Windows 64-bit).

4. Once downloaded, unzip the file to your desired location.

5. Run `GPTOrganizer.exe` (or the equivalent for your OS) from the extracted folder.

### Configuration

As with the source version, before running the tool, make sure to set up your OpenAI API Key:

1. Pass it as a command-line option:

    ```bash
    GPTOrganizer.exe --api-key YOUR_API_KEY
    ```

2. Or, set an environment variable named `OPENAI_API_KEY` with your key as its value.

## Obtaining an API Key from OpenAI:

1. **Create an OpenAI Account**:
   - If you don't have an OpenAI account, you'll first need to sign up. Go to the [OpenAI homepage](https://www.openai.com/) and click on `Sign Up` or `Join`.

2. **Navigate to the Dashboard**:
   - Once you have an account and are logged in, go to the OpenAI dashboard, which is typically available at `https://platform.openai.com/`.

3. **Access the API Section**:
   - In the dashboard, find and click on the `API` section.

4. **Create a New API Key**:
   - Click on `Create New Key` or a similar button that suggests generating a new API key.
   - Give your key a name or description if prompted. This can help you remember its purpose later on, especially if you have multiple keys.

5. **Copy the API Key**:
   - Once the key is generated, it will be displayed to you. Ensure you copy it immediately and store it safely. For security reasons, the exact key value might not be shown to you again.

6. **Keep Your Key Confidential**:
   - Treat your API key like a password. Do not share it publicly or push it to public repositories. Always use environment variables or configuration files that are not tracked by version control systems to store sensitive information.

7. **Set Limits (Optional)**:
   - OpenAI might provide you options to set usage limits on your API key to prevent accidental overuse. Set them as per your requirements.

8. **Check the Documentation**:
   - It's always a good idea to familiarize yourself with OpenAI's documentation to understand rate limits, costs, and best practices.
   
## Usage

To start using the tool, navigate to the directory where `GPTOrganizer` is located, and execute:

```bash
GPTOrganizer.exe -k YOUR_API_KEY -d DIRECTORY_PATH
```

Replace `YOUR_API_KEY` with your OpenAI API key, and `DIRECTORY_PATH` with the path to the directory you'd like to organize.
Files will be copied or moved depending on flag specified to the main folder "GPTOrganizer" within same location as target folder for organization.

## Options

- `-k, --api-key`: 
  - OpenAI API Key. If not provided, the program will attempt to use the `OPENAI_API_KEY` environment variable.
  
- `-d, --directory`: 
  - Directory containing the files to be organized. **(Required)**
  
- `-c, --categories`: 
  - Path to the `Categories.json` file. Default is based on the location of the .exe file.
  
- `-m, --move`: 
  - Move files instead of copying. Default is `false` (copy).
  
- `-l, --log-level`: 
  - Set the minimum log level. Options include `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical`. Default is `Information`.
  
- `-t, --timeout`: 
  - Timeout for the API call in seconds. Default is `300 seconds` (5 minutes).
  
- `-r, --retries`: 
  - Maximum number of retries for the API call. Default is `5`.
  
- `-e, --endpoint`: 
  - Specify the OpenAI API endpoint. Default is `https://api.openai.com/v1/chat/completions`.
  
- `-o, --model`: 
  - Specify the OpenAI model to be used. Options include: 
    - `gpt-3.5-turbo`
    - `gpt-3.5-turbo-0301`
    - `gpt-3.5-turbo-0613`
    - `gpt-3.5-turbo-16k`
    - `gpt-3.5-turbo-16k-0613`
    - `gpt-4`
    - `gpt-4-0314`
    - `gpt-4-0613`
  - Default is `gpt-3.5-turbo`.
  
- `-h, --help`: 
  - Display the help message.
```
