use serde::{Deserialize, Serialize};
use yew::{html, Html};
use yew_router::Routable;

use crate::pages::{
    about::About, account::Account, home::Home, insights::Insights, post_add::AddPost,
    posts::Posts, sign_in::SignIn, sign_out::SignOut, sign_up::SignUp,
};

#[derive(Clone, Routable, PartialEq)]
pub enum Route {
    #[at("/")]
    Home,
    #[at("/posts")]
    Posts,
    #[at("/about")]
    About,
    #[at("/account")]
    Account,
    #[at("/insights")]
    Insights,
    #[at("/signin")]
    SignIn,
    #[at("/signup")]
    SignUp,
    #[at("/signout")]
    SignOut,
    #[at("/addpost")]
    AddPost,
}

pub fn switch(routes: Route) -> Html {
    match routes {
        Route::Home => html! {<Home />},
        Route::Posts => html! {<Posts />},
        Route::AddPost => html! {<AddPost />},
        Route::About => html! {<About />},
        Route::Account => html! {<Account />},
        Route::Insights => html! {<Insights />},
        Route::SignIn => html! {<SignIn />},
        Route::SignUp => html! {<SignUp />},
        Route::SignOut => html! {<SignOut />},
    }
}

#[derive(Serialize, Deserialize)]
pub struct PostQuery {
    pub page: u32,
}
