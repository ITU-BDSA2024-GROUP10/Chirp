async function toggleLike(button, cheepId) {
    const csrfToken = document.querySelector('meta[name="csrf-token"]').getAttribute('content');
    const isLiking = !button.classList.contains('liked');

    const endpoint = isLiking ? '/like?handler=Like' : '/like?handler=Unlike';
    console.log(`Toggle like for Cheep ID: ${cheepId}, IsLiking: ${isLiking}`);

    try {
        const response = await fetch(endpoint, {
            method: 'POST',
            headers: { 
                'Content-Type': 'application/x-www-form-urlencoded',
                'RequestVerificationToken': csrfToken 
            },
            body: `cheepId=${encodeURIComponent(cheepId)}`
        });

        if (response.ok) {
            const data = await response.json();
            button.innerText = `Like (${data.likeCount})`;
            button.classList.toggle('liked', isLiking);
        } else {
            console.error('Failed to toggle like:', response.statusText);
            alert('Failed to update like status');
        }
    } catch (error) {
        console.error('Error toggling like:', error);
        alert('An error occurred while toggling like');
    }
}