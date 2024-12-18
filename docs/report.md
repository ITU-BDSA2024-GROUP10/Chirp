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
enabling logging of which Authors have interacted with which Cheeps. Furthermore Authors are able to follow other Authors, 
storing a list Authors they follow and Authors that follow them.

![Illustration of the _Chirp!_ data model as UML class diagram.](images/DomainModel.png)

\pagebreak

## Architecture — In the small

Due to the application's size, each layer consists only of a single project, as highlighted in bold. **Chirp.Web** references **Chirp.Infrastructure**, which deviates from the Onion architecture, for two reasons:

1. **Program.cs** requires it to configure services.

2. ASP.NET Identity uses it for user registration and verification.

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

- Be able to build 

- Have no failing test 
was pulled into main, the following workflow structure was setup. 

![Illustration of github workflows for pull requests into the main branch of _Chirp!_ ](images/PullRequests.png){ width=75%}

![Workflow structure for pull requests into the main branch of _Chirp!_ ](images/PR_Workflow.png)

\pagebreak

## Team work
### Project Board

### Unclosed Issues
Some issues still remain open in the Todo column, these are extra features that the group found interesting but did not get to implement within the time frame of the project work.
&nbsp;

In order to better mimic the functionality of _X_ (f.k.a _Twitter_), users should be able to leave comments directly on the timeline pages. 
This would be implemented by having a popup window appear, where users could leave comments, when clicking a Cheep. 
However getting this to work while handling and displaying message-format-errors proved to be an issue, and the feature was given an _Extra_ tag and left open. 
&nbsp;

We also wanted to make a big refactor, which involved moving what database access we could to an API project. Since we use ASP.NET Identity for user registration and verification, a local database would still be required for the web project to store user information. The main reason for the API project is to decouple data access from the web application, making it easier to build additional features, such as a mobile app, by enabling shared data across projects. While a centralized database could achieve similar results, an API is more future-proof, as it abstracts the database layer, making the switching of the database, have no impact on the projects using the API.

### Issue Progression 
The illustration below shows how the group worked with issues during the project. 
Steps highlighted in blue show issue creation, red boxes show the development process
and green how issues are merged from the feature branch into main.

![Illustration of the _Chirp!_ issue progression from creation to merge.](images/FromIssueToMerged.png)

\pagebreak

## How to make _Chirp!_ work locally
First clone the repository to your machine with 

\vspace*{0.5cm}
```
git clone https://github.com/ITU-BDSA2024-GROUP10/Chirp.git
```
\vspace*{0.5cm}

In order for the program to work you will need to configure the user-secrets. 
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
Next navigate to _/Chirp/Chirp/src/Chirp.Web_ and in your terminal do either:
\vspace*{0.5cm}
```
dotnet watch 
```


```
dotnet run 
```

## How to run test suite locally

In order to make run the UI-Tests make sure your system has playwright installed.

Next, navigate to _/Chirp/Chirp_ and in your terminal do
\vspace*{0.5cm}
```
dotnet test 
```

## Our test structure
We have three kinds of test

- Unit

- UI

- End to end

&nbsp;

Since our services are essentially return statements calling our repository, we found integration tests of these to be of lesser valued, compared to the rest of the project. However, if the project continues, testing them would be beneficial to ensure functionality remains unchanged as the services evolve.
\vspace*{0.3cm}

Most of the unit tests are on the repository methods since these are where almost all of the compute is being done. 

\vspace*{0.3cm}
Our UI tests are set up quite general, we don't mock anything, just use an in memory database, and only validate if the UI behave as expected. 
You could, in addition to these, have done some tests where you isolate the UI more, e.g., by mocking the behavior of the used service methods. But we prioritized other tasks given the application's size.
We have some end to end test, but could probably use some more, since our UI test are so general, they act some what as end to end tests.

\pagebreak


# Ethics

## License

This program is licensed with the GPL-2.0 License

## LLMs, ChatGPT, CoPilot, and others

### CoPilot
CoPilot have been used doing the development of this project. It's a great tool for speeding up development, as it's quicker to read through the code it recommends than to write it. Not everything it recommends is usable or as we want it, but it can also help when learning a new language or framework, to introduce new methods and structure.

### ChatGPT
ChatGPT was used primarily for the three following things.

- Understanding and debugging error messages

- Writing some HTML and CSS code

- Understand and discussing

&nbsp;

**Understanding and debugging error messages.**
This can be very helpful since error messages can be very long and contain a lot of information, which is ideal for LLM's. 
Sometimes they can also be harder to understand if you don't have a lot of knowledge of the framework you are using. 
&nbsp;

\vspace*{0.3cm}

**Writing some HTML and CSS code.**
Since HTML and CSS is time consuming we used ChatGPT to help write some of the UI code, that could be fine tuned by hand.   
&nbsp;

**Understand and discussing code.**
If you don't have other people around, this can be very helpful, especially if you find some code online, whether it's from a stack overflow post, or if it's documentation. When learning a new language or framework, this can especially be helpful, since you don't know a lot of the tricks yet.  
&nbsp;

Overall, the use of LLM's speed-up our development process, and helped us get a better understanding of c# and dot net. 
While using LLM's can help speed up debugging and development processes, it is still important to learn how to work independent of AI-assistance.
The data centers running the models also consume large amount of energy, and as a developer you need to be conscious of the impact of this technology. 
