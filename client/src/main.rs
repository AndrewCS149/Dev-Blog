use yew::prelude::*;
use yew_router::prelude::*;

mod about;
mod components;
mod home;
mod nav;
mod posts;

use about::About;
use home::Home;
use nav::Nav;
use posts::Posts;

#[derive(Clone, Routable, PartialEq)]
enum Route {
    #[at("/")]
    Home,
    #[at("/posts")]
    Posts,
    #[at("/about")]
    About,
    #[not_found]
    #[at("/404")]
    NotFound,
}

fn switch_main(route: Route) -> Html {
    match route {
        Route::Home => html! {<Home/>},
        Route::Posts => html! {<Posts/>},
        Route::About => html! {<About/>},
        Route::NotFound => html! {<h1>{"Not Found"}</h1>},
    }
}

#[function_component(App)]
pub fn app() -> Html {
    html! {
        <BrowserRouter>
            <Nav/>
            <Switch<Route> render={switch_main} />
        </BrowserRouter>
    }
}

fn main() {
    yew::Renderer::<App>::new().render();
}
