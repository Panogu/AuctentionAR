## AuctentionAR - Auctioning Off Visual Attention in Mixed Reality

Pandjaitan, A., Strecker, J., Bektas, K., & Mayer, S. (2024, May). AuctentionAR-AuctioningOff Visual Attention in Mixed Reality. In _Extended Abstracts of the CHIConference on Human Factors in Computing Systems_ (pp. 1-6).

### What is AuctentionAR

![AuctentionAR Live](https://github.com/Panogu/AuctentionAR/blob/Main/Images/AuctentionAR_live.png?raw=true)

Mixed Reality technologies are increasingly interwoven with our everyday lives. A variety of powerful Head Mounted Displays have recently entered consumer electronics markets, and more are under development, opening new dimensions for spatial computing. This development will likely not stop at the advertising industry either, as first forays into this area have already been made.
We present AuctentionAR which allows users to sell off their visual attention to interested parties. It consists of a HoloLens 2, a remote server executing the auctioning logic, the YOLOv7 model for image recognition of products which may induce an advertising intent, and several bidders interested in advertising their products. As this system comes with substantial privacy implications, we discuss what needs to be considered in future implementation so as to make this system a basis for a privacy preserving MR advertising future.

### How to use this repository

This repository contains the server (NodeJS) and client (Unity) code to run the AuctentionAR system. The client code is designed to be run on a Microsoft HoloLens 2 with [MRTK 2](https://learn.microsoft.com/en-us/windows/mixed-reality/mrtk-unity/mrtk2/).

To use the server, you need to have NodeJS with NPM installed, then if you open a terminal in the server folder, just run `npm install` and then `npm run start` to start a server on your local machine.

The client is a bit more complex as you need to build it yourself (see information to the MRTK 2 above). In Unity you should change the IP address where the HL2 should connect to (i.e., the address of your local machine where the server is running) - there the gaze information will be uploaded.
![AuctentionAR Diagram](https://github.com/Panogu/AuctentionAR/blob/Main/Images/Unity_settings.png?raw=true)

### Overview of the system

![AuctentionAR Diagram](https://github.com/Panogu/AuctentionAR/blob/Main/Images/AuctentionAR_diagram.png?raw=true)