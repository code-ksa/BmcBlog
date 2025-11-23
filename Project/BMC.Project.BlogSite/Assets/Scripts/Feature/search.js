// Search Feature Scripts

export function initializeSearchFeatures() {
    initializeLiveSearch();
    initializeSearchForm();
    initializeSearchFilters();
}

// Initialize Live Search with Autocomplete
function initializeLiveSearch() {
    let searchTimeout;
    const searchInput = $('#searchInput');
    const autocompleteContainer = $('#searchAutocomplete');

    searchInput.on('keyup', function() {
        clearTimeout(searchTimeout);
        const query = $(this).val().trim();

        if (query.length >= 3) {
            searchTimeout = setTimeout(function() {
                performLiveSearch(query);
            }, 500);
        } else {
            autocompleteContainer.hide();
        }
    });

    // Close autocomplete when clicking outside
    $(document).on('click', function(e) {
        if (!$(e.target).closest('.search-box').length) {
            autocompleteContainer.hide();
        }
    });
}

// Perform Live Search via AJAX
function performLiveSearch(query) {
    $.ajax({
        url: '/Search/QuickSearch',
        method: 'GET',
        data: { q: query },
        success: function(results) {
            displayAutocompleteResults(results);
        },
        error: function() {
            console.error('Live search failed');
        }
    });
}

// Display Autocomplete Results
function displayAutocompleteResults(results) {
    const container = $('#searchAutocomplete');
    container.empty();

    if (results && results.length > 0) {
        results.forEach(function(result) {
            const item = $('<div>', {
                class: 'autocomplete-item',
                html: `
                    <div class="item-title">${result.title}</div>
                    <div class="item-snippet">${result.snippet}</div>
                `
            });

            item.on('click', function() {
                window.location.href = result.url;
            });

            container.append(item);
        });
        container.show();
    } else {
        container.html('<div class="no-results">No results found</div>').show();
    }
}

// Initialize Search Form
function initializeSearchForm() {
    $('#searchForm').on('submit', function(e) {
        const query = $('#searchInput').val().trim();
        if (query === '') {
            e.preventDefault();
            alert('Please enter a search term');
        }
    });
}

// Initialize Search Filters
function initializeSearchFilters() {
    $('.search-filter').on('change', function() {
        const filterType = $(this).data('filter');
        const filterValue = $(this).val();
        
        // Update search results based on filter
        updateSearchResults(filterType, filterValue);
    });
}

// Update Search Results
function updateSearchResults(filterType, filterValue) {
    const currentUrl = new URL(window.location.href);
    currentUrl.searchParams.set(filterType, filterValue);
    window.location.href = currentUrl.toString();
}

// Export functions
export default {
    initializeSearchFeatures
};