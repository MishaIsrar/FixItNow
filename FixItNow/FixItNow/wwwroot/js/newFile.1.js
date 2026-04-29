$(document).ready(function() {
    $('#searchButton').on('click', function() {
        let query = $('#searchInput').val(); // Get the value from the input field


        // Clear results if input is empty
        if (query.trim() === '') {
            $('#searchResults').empty();
            return;
        }

        // Send AJAX request
        $.ajax({
            url: '/Services/SearchByTitle',
            type: 'GET',
            data: { title: query },
            success: function(data) {
                $('#searchResults').empty();
                $('#searchResults').html(data); // Populate results using the partial view
            },
            error: function(err) {
                console.error(err);
            }
        });
    });
});
