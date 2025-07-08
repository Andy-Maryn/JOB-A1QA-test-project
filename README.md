
# ReplaceBook
Test case 1 - Search and navigate to the Steam About page

Preconditions:
- The browser should open in incognito mode.
- Go to https://store.steampowered.com

| Steps | Expected Results |
|-------|------------------|
| 1 Type 'FIFA' inthe search field | The first search result is 'EA SPORTS FCTM 25', The second search result is 'FIFA 22'|
| 2 Click the first search result using a JavaScript script | The Game page is displayed, The game name equals the game name from the 1st search result|
|3 Click Download button||
|4 Click No, I need Steam button| About Steam page is displayed, Install Steam button is clickable, Playing Now gamers are less than Online gamers|
