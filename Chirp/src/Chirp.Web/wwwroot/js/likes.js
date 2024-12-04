async function toggleLike(button, cheepId, isLiking) {
    const csrfToken = document.querySelector('meta[name="csrf-token"]').getAttribute('content');

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
            button.innerText = `${newIsLiking ? 'Like' : 'Unlike'} (${likeCount})`;
            button.setAttribute('onclick', `toggleLike(this, ${cheepId}, ${newIsLiking})`);
        } else {
            alert('Failed to update like status');
        }
    } catch (error) {
        console.error('Failed to toggle like:', error);
    }
}