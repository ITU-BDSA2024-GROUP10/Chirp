---
title: _Chirp!_ Project Report
subtitle: ITU BDSA 2024 Group `10`
author:
- "Christoffer Grünberg <gryn@itu.dk>"
- "Rasmus Rosenmejer Larsen <rarl@itu.dk>"
- "Mathias Labori Olsen <mlao@itu.dk>"
- "Alex Tilgreen Mogensen <alext@itu.dk>"
- "Anthon Castillo Hertzum <acah@itu.dk>"
- "Bryce Raj Karnikar <brka@itu.dk>"
numbersections: true
header-includes:
  - \usepackage{caption}
  - \usepackage{graphicx}
---
\begin{center}
\includegraphics[width=0.5\textwidth]{images/icon1.png}
\end{center}

\pagebreak

# Design and Architecture of _Chirp!_

## Domain model

The _Chirp!_ domain model is setup around the Author class. Authors inherit traits for account management from IdentityUser. 
Authors are able to create Cheeps and interact with them with Likes or Comments. Each Author keeps a list of Likes and Comments
enabling logging of which Authors have interacted with which Cheeps.

![Illustration of the _Chirp!_ data model as UML class diagram.](images/DomainModel.png)

\pagebreak

## Architecture — In the small

Due to the application's size, each layer consists only of a single project, as highlighted in bold. **Chirp.Web** references **Chirp.Infrastructure**, which deviates from the Onion architecture, for two reasons:
1. **Program.cs** requires it to configure services.
2. Microsoft Identity uses it for user registration and verification.

![Illustration of the _Chirp!_ program architecture.](images/OnionModel.png)

\pagebreak

## Architecture of deployed application

The _Chirp!_ application is deployed to the Microsoft Azure App Service as a complete component 
consisting of Chirp.Web for the GUI, Chirp.Infrastructure handling the domain model and repositories.
The User connects to Chirp.Web through Azure. On read and write requests the Azure Web App will
make calls to the deployed SQLite server. If users attempt to login or register with OAuth via github
Chirp.Web will make calls to GitHub Authentication. 

![Illustration of the _Chirp!_ deployment architecture of the application.](images/DeploymentDiagram.png)

## User activities

In order to increase the readability of the UserActivities diagram, the total diagram has been decomposed 
to show activities depending on whether the User is signed in or not. 

The total diagram can be under _docs/images/UserActivitiesDiagram.png_

\pagebreak

\vspace*{-2cm}
\noindent
\hspace*{-2cm}
\makebox[\textwidth][l]{%
    \includegraphics[width=1\textwidth, height=\textheight, keepaspectratio]{images/LoggedOut.png}
}

\vspace*{0.2cm}
\captionof{figure}{Illustration of the \textit{Chirp!} functionality while signed out.}
\vspace*{2cm}
\vspace*{-2cm}
\noindent
\hspace*{-2cm}
\makebox[\textwidth][l]{%
    \includegraphics[width=1\textwidth, height=\textheight, keepaspectratio]{images/LoggedIn.png}
}

\vspace*{0.5cm}
\captionof{figure}{Illustration of the \textit{Chirp!} functionality while signed in.}

\pagebreak

## Sequence of functionality/calls trough _Chirp!_

\noindent
\hspace*{-2cm}
\makebox[\textwidth][l]{%
    \includegraphics[width=1.4\textwidth, height=\textheight, keepaspectratio]{images/Sequence-of-functionality.png}
}
\vspace*{0.5cm}
\captionof{figure}{Sequence diagram of the flow of messages through the \textit{Chirp!} application.}


\pagebreak 

# Process


## Build, test, release, and deployment


The figure 5 below illustrates the workflows used for building and deploying the _Chirp!_ application.
The process stars, when pull request is merged into the main branch.
The blue boxes represents workflows


![Illustration of github workflows for building and deploying the _Chirp!_ application.](images/Deployment.png)

\vspace*{-2cm}
\noindent
\hspace*{-2cm}
\makebox[\textwidth][l]{%
    \includegraphics[width=1.2\textwidth, height=\textheight]{images/ReleaseWorkflows.png}
}
\vspace*{-1cm}
\vspace*{0.5cm}
\captionof{figure}{Detailed illustration of the workflows involved in deploying \textit{Chirp!}.}

\pagebreak

## Pull Requests 

To help validate pull requests and help make sure only code, that lived up to the following, 
- Contain no warnings
- Be abel to build 
- Have no failing test 
was pulled into main, the following workflow structure was setup. 

![Illustration of github workflows for building and deploying the _Chirp!_ application.](images/PullRequests.png)


## Team work
### Project Board

### Unclosed Issues
Some issues still remain open in the Todo column, these are extra features that the group found interesting but did not get to implement within the time frame of the project work.

In order to better mimic the functionality of _X_ (f.k.a _Twitter_), users should be able to leave comments directly on the timeline pages. 
This would be implemented by having a popup window appear, where users could leave comments, when clicking a Cheep. 
However getting this to work while handling and displaying message-format-errors proved to be an issue, and the feature was given an _Extra_ tag and left open. 

We also wanted to make a big refactor, which involved moving what database access we could to an API project. Since we use Microsoft Identity for user registration and verification, a local database would still be required for the web project to store user information. The main reason for the API project is to decouple data access from the web application, making it easier to build additional features, such as a mobile app, by enabling shared data across projects. While a centralized database could achieve similar results, an API is more future-proof, as it abstracts the database layer, making the switching of the database, have no impact on the projects using the API.

### Issue Progression 
The illustration below shows how the group worked with issues during the project. 
Steps highlighted in blue show issue creation, red boxes show the development process
and green how issues are merged from the feature branch into main.

![Illustration of the _Chirp!_ issue progression from creation to merge.](images/FromIssueToMerged.png)

\pagebreak

## How to make _Chirp!_ work locally

In order for the program to work you will first need to configure the user-secrets. 
To do this navigate to  _/Chirp/Chirp_, in the terminal, and run:

\vspace*{0.5cm}
Windows:

```
dotnet user-secrets init --project .\src\Chirp.Web\
dotnet user-secrets set "authentication:github:clientId" "Ov23lisGJEMdXORhzpDr" 
--project .\src\Chirp.Web\

dotnet user-secrets set 
"authentication:github:clientSecret" "a9229ceee8bb014070dc9abe892cf07d7aba4d0d" 
--project .\src\Chirp.Web\

```

\vspace*{0.5cm}
MacOs & Linux:


```
dotnet user-secrets init --project ./src/Chirp.Web/
dotnet user-secrets set "authentication:github:clientId" "Ov23lisGJEMdXORhzpDr" 
--project ./src/Chirp.Web/

dotnet user-secrets set 
"authentication:github:clientSecret" "a9229ceee8bb014070dc9abe892cf07d7aba4d0d" 
--project ./src/Chirp.Web/

```
\vspace*{0.5cm}
Next navigate to _/Chirp/Chirp/src/Chirp.Web_ and in your terminal either of:
\vspace*{0.5cm}
```
dotnet watch 
```


```
dotnet run 
```

## How to run test suite locally

Navigate to _/Chirp/Chirp_ and in your terminal do _dotnet test_.

# Ethics

## License

This program is licensed with the GPL-2.0 License

## LLMs, ChatGPT, CoPilot, and others

ChatGPT was used to understand and debug error messages, and write some html for the razor pages. 
