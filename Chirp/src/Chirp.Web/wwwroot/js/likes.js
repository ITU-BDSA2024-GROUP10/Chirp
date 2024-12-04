async function toggleLike(button, cheepId, isLiking) {
    const csrfToken = document.querySelector('meta[name="csrf-token"]').getAttribute('content');
    const likeCountElement= document.getElementById(`likecount-${cheepId}`);

    try {
        const response = await fetch('/like?handler=ToggleLike', {
            method: 'POST',
            headers: { 
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': csrfToken 
            },
            body: `cheepId=${encodeURIComponent(cheepId)}&isLiking=${isLiking}`
        });

        if (response.ok) {
            const data = await response.json();
            const likeCount = data.likeCount;
            isLiking = isLiking === 'true' || isLiking === true;
            const newIsLiking = !isLiking;
            button.setAttribute('onclick', `toggleLike(this, ${cheepId}, ${newIsLiking})`);
            button.setAttribute('class', `like-button${isLiking ? "-liked" : ""}`);
            likeCountElement.textContent = likeCount;
        } else {
            alert('Failed to update like status');
        }
    } catch (error) {
        console.error('Failed to toggle like:', error);
    }
}
