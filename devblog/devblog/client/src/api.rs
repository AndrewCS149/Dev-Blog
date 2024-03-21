use gloo::console::log;
use gloo_net::http::{Headers, Method, RequestBuilder, Response};
use wasm_bindgen::JsValue;

const URL: &str = "https://localhost:44482/api/";

#[derive(Clone, PartialEq)]
pub enum Api {
    AddComment,
    AddPost,
    DeleteAccount(String),
    DeleteComment(u32),
    DeleteCurrentAccount,
    DeletePost(u32),
    DeleteNotification(u32, String),
    EditComment(u32),
    EditPost(u32),
    GetPage(u32),
    GetPagesCount,
    GetPageNumber(u32),
    GetPost(i32),
    GetPostsCount,
    GetCurrentUser,
    GetNotifications(String),
    GetUsers,
    GetVideoUrl,
    SignIn,
    SignOut,
    SignUp,
    ToggleSubscribe,
    UpdateVideoUrl,
    Vote(u32, String),
}

impl Api {
    pub async fn fetch(
        &self,
        hdrs: Option<Headers>,
        body: Option<JsValue>,
        method: Method,
    ) -> Option<Response> {
        let request_builder = RequestBuilder::new(&self.uri())
            .headers(hdrs.unwrap_or_default())
            .method(method)
            .body(body.unwrap_or_default());

        match request_builder {
            Ok(req) => {
                let req_result = req.send().await;
                match req_result {
                    Ok(response) => Some(response),
                    Err(e) => {
                        log!("Error sending request: ", e.to_string());
                        None
                    }
                }
            }
            Err(e) => {
                log!("Error building request: ", e.to_string());
                None
            }
        }
    }

    fn uri(&self) -> String {
        match self {
            Api::AddComment => format!("{}comments", URL),
            Api::AddPost => format!("{}posts", URL),
            Api::DeleteAccount(username) => format!("{}accounts/adminDelete/{}", URL, username),
            Api::DeleteComment(id) => format!("{}comments/{}", URL, id),
            Api::DeleteCurrentAccount => format!("{}accounts", URL),
            Api::DeletePost(id) => format!("{}posts/{}", URL, id),
            Api::DeleteNotification(post_id, username) => {
                format!("{}notifications/{}/{}", URL, post_id, username)
            }
            Api::EditComment(id) => format!("{}comments/{}", URL, id),
            Api::EditPost(id) => format!("{}posts/{}", URL, id),
            Api::GetPage(num) => format!("{}posts?page={}", URL, num),
            Api::GetPagesCount => format!("{}posts/countPages", URL),
            Api::GetPageNumber(id) => format!("{}posts/getPageNum/{}", URL, id),
            Api::GetPost(id) => format!("{}posts/{}", URL, id),
            Api::GetPostsCount => format!("{}posts/countPosts", URL),
            Api::GetCurrentUser => format!("{}accounts/user", URL),
            Api::GetNotifications(username) => format!("{}notifications/{}", URL, username),
            Api::GetUsers => format!("{}accounts", URL),
            Api::GetVideoUrl => format!("{}YtVideo", URL),
            Api::SignIn => format!("{}accounts/signin", URL),
            Api::SignOut => format!("{}accounts/signout", URL),
            Api::SignUp => format!("{}accounts/signup", URL),
            Api::ToggleSubscribe => format!("{}accounts/subscribe", URL),
            Api::UpdateVideoUrl => format!("{}YtVideo/1", URL),
            Api::Vote(id, vote) => format!("{}posts/{}/{}", URL, id, vote),
        }
    }
}
