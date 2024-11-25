async function toggleFollow(button, followName, shouldFollow) {
    console.log(`Sending: followName=${followName}, shouldFollow=${shouldFollow}, returnUrl=${window.location.pathname}`);

    const response = await fetch('/follow', {
        method: 'POST',
        headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
        body: `followName=${followName}&shouldFollow=${shouldFollow}&returnUrl=${window.location.pathname}`
    });

    if (response.ok) {
        button.innerText = shouldFollow ? 'unfollow' : 'follow';
        button.setAttribute('onclick', `toggleFollow(this, '${followName}', ${!shouldFollow})`);
    } else {
        alert('Failed to update follow status');
    }
}