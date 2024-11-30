//Even tho it says "export default" is unused, it is needed to work, 
//since it used for import in default.cshtml, for cheepList
export default class VideoController {
    constructor(videoElement, videoList) {
        this.video = videoElement;

        this.videoList = videoList;

        this.videoIndex = 0;

        this.init();
    }

    init() {
        this.setupVideoListeners(this.video, this.videoList);

        this.disableContextMenu(this.video);
    }

    setupVideoListeners(videoElement, videoList) {
        videoElement.addEventListener('ended', () => {
            this.playNextVideo(videoElement, videoList);
        });

        videoElement.addEventListener('loadeddata', () => {
            videoElement.play();
        });
        
        videoElement.addEventListener('contextmenu', (event) => {
            event.preventDefault();
        });
    }

    playNextVideo(videoElement, videoList) {
        this.videoIndex = (this.videoIndex + 1) % videoList.length;
        videoElement.src = videoList[this.videoIndex];
        videoElement.load();
    }

    disableContextMenu(videoElement) {
        videoElement.addEventListener('contextmenu', (event) => {
            event.preventDefault();
        });
    }
}