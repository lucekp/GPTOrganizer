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

Replace `lucekp` with your GitHub username.

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

## Usage

To start using the tool, navigate to the directory where `GPTOrganizer` is located, and execute:

```bash
GPTOrganizer.exe -k YOUR_API_KEY -d DIRECTORY_PATH
```

Replace `YOUR_API_KEY` with your OpenAI API key, and `DIRECTORY_PATH` with the path to the directory you'd like to organize.

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
