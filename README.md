# MicrosoftSemanticKernel-Samples

The repository named MicrosoftSemanticKernelSamples contains code samples featured in my video tutorials aimed at helping you understand Microsoft Semantic Kernel. 
To acquire knowledge about Microsoft Semantic Kernel, please consider visiting [Microsoft Semantic Kernel](https://www.youtube.com/@vinothrajendran)
.

## 01-HelloWorld
In this example, we illustrate a Hello World application using the Semantic Kernel. We demonstrate how to construct a prompt, establish a connection to Azure OpenAI services, process the user's input, and finally, display the resulting output after execution.

[Source code here](https://github.com/rvinothrajendran/MicrosoftSemanticKernelSamples/tree/main/SKSampleCSharp/01HelloWorld)


### Video tutorials are available here
[![Watch the Demo](https://img.youtube.com/vi/-lPI4DNKDWc/0.jpg)](https://www.youtube.com/watch?v=-lPI4DNKDWc&t=0s)

## 02-Prompt plugins
This example illustrates the utilization of a Plugin through Prompt functions in the Microsoft Semantic Kernel. The Prompt function is specified in both the config.json and skprompt.txt files.

[Source code here](https://github.com/rvinothrajendran/MicrosoftSemanticKernelSamples/tree/main/SKSampleCSharp/PromptFunction)

### Ref Video tutorials
[![Watch the Demo](https://img.youtube.com/vi/Hyfh1wf2QjM/0.jpg)](https://www.youtube.com/watch?v=Hyfh1wf2QjM)

## 03-YAML Config 
The example highlights a different approach to creating a prompt plugin within the Microsoft Semantic Kernel using YAML configuration. Unlike the conventional method that involves using two separate files, namely Config.json and SKPrompt.txt, to define the prompt template, this alternative approach condenses the process by consolidating the template creation into a single file. This simplification can enhance readability and manageability in certain scenarios.

[Source code here](https://github.com/rvinothrajendran/MicrosoftSemanticKernelSamples/tree/main/SKSampleCSharp/PromptYAMLDemo)

### Ref Video tutorials
[![Watch the Demo](https://img.youtube.com/vi/1mvb3wZg8JQ/0.jpg)](https://www.youtube.com/watch?v=1mvb3wZg8JQ)

## 09-HandleBarsTemplate 
Handlebars functions as a template engine, empowering the development of dynamic and reusable prompts for large language models (LLMs) within the Semantic Kernel. The Semantic Kernel serves as a platform specifically crafted for building generative AI applications. By harnessing the capabilities of Handlebars, users have the flexibility to utilize variables, expressions, helpers, and partials to customize prompts, thereby augmenting their adaptability and effectiveness. The C# SDK of the Semantic Kernel seamlessly integrates with Handlebars, and the HandlebarsPromptTemplateFactory class streamlines the creation and management of Handlebars templates.

[Source code here](https://github.com/rvinothrajendran/MicrosoftSemanticKernelSamples/tree/main/SKSampleCSharp/HandleBarsPromptTemplate)

### Ref
[![Watch the Demo](https://img.youtube.com/vi/iw4jlKdq4qA/0.jpg)](https://www.youtube.com/watch?v=iw4jlKdq4qA)
